using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace CommentMap.EventBus.Logging;

internal static partial class EventBusLogs
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning,
        Message = "Could not find event type for name '{EventName}' (message body: '{MessageBody}').")]
    public static partial void LogMessageNotRecognized(this ILogger logger, string eventName, string messageBody);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning,
        Message = "Failed to process message '{EventName}', nacking with requeue.")]
    public static partial void LogMessageProcessingFailed(this ILogger logger, string eventName, Exception? exception);
}

internal static class ActivityExtensions
{
    public static void SetExceptionTags(this Activity activity, Exception ex)
    {
        if (activity.IsAllDataRequested)
        {
            activity.AddTag("exception.type", ex.GetType().FullName);
            activity.AddTag("exception.message", ex.Message);

            if (ex.StackTrace is not null)
            {
                activity.AddTag("exception.stacktrace", ex.StackTrace);
            }
        }
    }
}
