using System.Text.Json;

namespace CommentMap.EventBus.Abstractions;

public class EventBusSubscriptionInfo
{
    public Dictionary<string, Type> EventTypes { get; } = [];

    public JsonSerializerOptions JsonSerializerOptions { get; } = new(JsonSerializerDefaults.Web);
}
