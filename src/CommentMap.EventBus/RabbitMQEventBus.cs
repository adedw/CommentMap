using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using CommentMap.EventBus.Abstractions;
using CommentMap.EventBus.Logging;
using CommentMap.Shared.Messages;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

using Polly;
using Polly.Retry;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace CommentMap.EventBus;

public sealed class RabbitMQEventBus(
    ILogger<RabbitMQEventBus> logger,
    IServiceProvider serviceProvider,
    IOptions<EventBusOptions> eventBusOptions,
    IOptions<EventBusSubscriptionInfo> subscriptionOptions,
    RabbitMQTelemetry rabbitMQTelemetry) : IEventBus, IDisposable, IHostedService
{
    private const string ExchangeName = "commentmap_event_bus";

    private readonly ResiliencePipeline _pipeline = CreateResiliencePipeline(eventBusOptions.Value.RetryCount);
    private readonly string _queueName = eventBusOptions.Value.SubscriptionClientName;
    private readonly EventBusSubscriptionInfo _subscriptionInfo = subscriptionOptions.Value;
    private readonly TextMapPropagator _propagator = rabbitMQTelemetry.Propagator;

    private IConnection? _rabbitMQConnection;
    private IChannel? _consumerChannel;

    private Task _consumingTask = Task.CompletedTask;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _consumingTask = Task.Run(async () =>
        {
            try
            {
                logger.LogInformation("Starting RabbitMQ event bus consumer.");

                _rabbitMQConnection = serviceProvider.GetRequiredService<IConnection>();

                _consumerChannel = await _rabbitMQConnection.CreateChannelAsync();

                await _consumerChannel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

                await _consumerChannel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct);

                await _consumerChannel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false);

                foreach (var eventName in _subscriptionInfo.EventTypes.Keys)
                {
                    await _consumerChannel.QueueBindAsync(_queueName, ExchangeName, routingKey: eventName);
                }

                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.ReceivedAsync += OnMessageReceived;

                await _consumerChannel.BasicConsumeAsync(_queueName, autoAck: false, consumer);

                logger.LogInformation("RabbitMQ event bus consumer started.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RabbitMQ event bus consumer failed to start.");
            }
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event)
        where TIntegrationEvent : IntegrationEvent
    {
        var eventName = @event.GetType().Name;

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Publishing integration event: {EventName} ({EventId})", eventName, @event.Id);
        }

        var body = SerializeMessage(@event);

        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            ContentType = "application/json",
        };

        await _pipeline.Execute(async () =>
        {
            using var channel = await (_rabbitMQConnection?.CreateChannelAsync()
                ?? throw new InvalidOperationException("RabbitMQ connection is not open"));

            await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct);

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: body);
        });
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);
        }

        if (!_subscriptionInfo.EventTypes.TryGetValue(eventName, out var eventType))
        {
            logger.LogMessageNotRecognized(eventName, message);
            return;
        }

        try
        {
            var @event = DeserializeMessage(message, eventType, _subscriptionInfo.JsonSerializerOptions)
                ?? throw new JsonException($"Deserialized event of type '{eventType.Name}' is null.");

            await using var scope = serviceProvider.CreateAsyncScope();

            foreach (var handler in scope.ServiceProvider.GetKeyedServices<IIntegrationEventHandler>(eventType))
            {
                await HandleWithTracing(handler, @event, eventArgs, eventName);
            }

            await _consumerChannel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            logger.LogMessageProcessingFailed(eventName, ex);
            Activity.Current?.SetExceptionTags(ex);

            // Requeue the message so it can be retried.
            // NOTE: a poisoned message will loop forever; a DLX is the production-grade fix (see todos.md).
            await _consumerChannel!.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
        }
    }

    private async Task HandleWithTracing(
        IIntegrationEventHandler handler,
        IntegrationEvent @event,
        BasicDeliverEventArgs eventArgs,
        string eventName)
    {
        var parentContext = _propagator.Extract(
            default, eventArgs.BasicProperties, (props, key) => ExtractTraceContextFromBasicProperties(props, key));

        using var activity = rabbitMQTelemetry.ActivitySource.StartActivity(
            $"eventbus {eventName}", ActivityKind.Consumer, parentContext.ActivityContext);

        await handler.Handle(@event, CancellationToken.None);
    }

    private static IEnumerable<string> ExtractTraceContextFromBasicProperties(
        IReadOnlyBasicProperties? properties, string key)
    {
        if (properties?.Headers is not { } headers || !headers.TryGetValue(key, out var value) || value is not byte[] bytes)
        {
            return [];
        }

        return [Encoding.UTF8.GetString(bytes)];
    }

    private byte[] SerializeMessage<TIntegrationEvent>(TIntegrationEvent @event)
        where TIntegrationEvent : IntegrationEvent
    {
        using var activity = rabbitMQTelemetry.ActivitySource.StartActivity(
            $"eventbus publish {@event.GetType().Name}", ActivityKind.Producer);

        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            ContentType = "application/json",
        };

        if (activity is not null)
        {
            _propagator.Inject(
                new PropagationContext(activity.Context, Baggage.Current),
                properties,
                InjectTraceContextIntoBasicProperties);
        }

        return JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), _subscriptionInfo.JsonSerializerOptions);
    }

    private static void InjectTraceContextIntoBasicProperties(IReadOnlyBasicProperties? props, string key, string value)
    {
        if (props is not BasicProperties writable)
        {
            return;
        }

        writable.Headers ??= new Dictionary<string, object?>();
        writable.Headers[key] = Encoding.UTF8.GetBytes(value);
    }

    private static IntegrationEvent? DeserializeMessage(string message, Type eventType, JsonSerializerOptions options)
        => (IntegrationEvent?)JsonSerializer.Deserialize(message, eventType, options);

    public void Dispose()
    {
        _consumerChannel?.Dispose();
        _consumingTask.Dispose();
    }

    private static ResiliencePipeline CreateResiliencePipeline(int retryCount)
    {
        var retryOptions = new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder()
                .Handle<BrokerUnreachableException>()
                .Handle<SocketException>(),
            MaxRetryAttempts = retryCount,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(1),
        };

        return new ResiliencePipelineBuilder()
            .AddRetry(retryOptions)
            .Build();
    }
}
