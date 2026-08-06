using CommentMap.EmailSender.Logging;
using CommentMap.Shared.Messages;

namespace CommentMap.EmailSender.Services;

public static class MessageSenderHandler
{
    public static async Task Handle(
        SendConfirmEmail message,
        IMessageSenderService emailMessageService,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var (email, callbackURL) = message;

        await emailMessageService.SendConfirmationLinkAsync(email, callbackURL, cancellationToken);

        loggerFactory.CreateLogger("MessageSenderHandler").LogConfirmationEmailSent(email);
    }

    public static async Task Handle(
        SendResetPasswordEmail message,
        IMessageSenderService emailMessageService,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var (email, callbackURL) = message;

        await emailMessageService.SendResetPasswordLinkAsync(email, callbackURL, cancellationToken);

        loggerFactory.CreateLogger("MessageSenderHandler").LogResetPasswordEmailSent(email);
    }

    public static async Task Handle(
        SendChangeEmail message,
        IMessageSenderService emailMessageService,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var (email, callbackURL) = message;

        await emailMessageService.SendEmailChangingLinkAsync(email, callbackURL, cancellationToken);

        loggerFactory.CreateLogger("MessageSenderHandler").LogEmailChangingLinkSent(email);
    }
}
