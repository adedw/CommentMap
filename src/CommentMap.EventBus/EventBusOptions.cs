namespace CommentMap.EventBus;

public class EventBusOptions
{
    public const string ConfigSectionName = "EventBus";

    /// <summary>
    /// Name of the queue this service consumes from.
    /// </summary>
    public string SubscriptionClientName { get; set; } = string.Empty;

    /// <summary>
    /// Number of retries when publishing a message before throwing.
    /// </summary>
    public int RetryCount { get; set; } = 10;
}
