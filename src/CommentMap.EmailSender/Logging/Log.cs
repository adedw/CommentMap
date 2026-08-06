using Mjml.Net;

namespace CommentMap.EmailSender.Logging;

internal static partial class Log
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "A confirmation email has been sent to '{Email}'.")]
    public static partial void LogConfirmationEmailSent(this ILogger loger, string email);

    
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "A reset password email has been sent to '{Email}'.")]
    public static partial void LogResetPasswordEmailSent(this ILogger loger, string email);
    
    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "A email changing link has been sent to '{Email}'.")]
    public static partial void LogEmailChangingLinkSent(this ILogger loger, string email);
}
