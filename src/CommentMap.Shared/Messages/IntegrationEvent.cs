using System.Text.Json.Serialization;

namespace CommentMap.Shared.Messages;

public abstract record IntegrationEvent
{
    [JsonInclude]
    public Guid Id { get; init; } = Guid.NewGuid();

    [JsonInclude]
    public DateTimeOffset CreationDate { get; init; } = DateTimeOffset.UtcNow;
}
