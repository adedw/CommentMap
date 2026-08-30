using System.Text.Json;

using CommentMap.EventBus.Abstractions;
using CommentMap.Shared.Messages;

using Microsoft.Extensions.DependencyInjection;

namespace CommentMap.EventBus;

public static class EventBusBuilderExtensions
{
    public static IEventBusBuilder AddSubscription<TIntegrationEvent, TIntegrationEventHandler>(this IEventBusBuilder builder)
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : class, IIntegrationEventHandler<TIntegrationEvent>
    {
        builder.Services.AddKeyedTransient<IIntegrationEventHandler, TIntegrationEventHandler>(typeof(TIntegrationEvent));

        builder.Services.Configure<EventBusSubscriptionInfo>(o =>
        {
            o.EventTypes[typeof(TIntegrationEvent).Name] = typeof(TIntegrationEvent);
        });

        return builder;
    }

    public static IEventBusBuilder ConfigureJsonOptions(this IEventBusBuilder builder, Action<JsonSerializerOptions> configure)
    {
        builder.Services.Configure<EventBusSubscriptionInfo>(o => configure(o.JsonSerializerOptions));
        return builder;
    }
}
