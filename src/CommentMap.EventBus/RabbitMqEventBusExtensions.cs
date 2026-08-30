using CommentMap.EventBus;
using CommentMap.EventBus.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class RabbitMqEventBusExtensions
{
    public static IEventBusBuilder AddEventBus(this IHostApplicationBuilder builder, string connectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(connectionName);

        builder.AddRabbitMQClient(connectionName);

        builder.Services.Configure<EventBusOptions>(builder.Configuration.GetSection(EventBusOptions.ConfigSectionName));

        builder.Services.AddSingleton<EventBusSubscriptionInfo>();
        builder.Services.AddSingleton<RabbitMQTelemetry>();
        builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
        builder.Services.AddSingleton<IHostedService>(sp => (RabbitMQEventBus)sp.GetRequiredService<IEventBus>());

        return new EventBusBuilder(builder.Services);
    }

    private sealed class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services { get; } = services;
    }
}
