using CommentMap.EmailSender.Logging;
using CommentMap.Shared.Messages;

namespace CommentMap.EmailSender.Services;

public class SendMessageConsumer(IMessageSenderService emailMessageService, ILogger<SendMessageConsumer> logger)
{
    public async Task ConsumeAsync(SendConfirmEmail sendConfirmEmail, CancellationToken cancellationToken)
    {
        var (email, callbackURL) = sendConfirmEmail;

        await emailMessageService.SendConfirmationLinkAsync(email, callbackURL, cancellationToken);

        logger.LogConfirmationEmailSent(email);
    }

    public async Task ConsumeAsync(SendResetPasswordEmail sendResetPasswordEmail, CancellationToken cancellationToken)
    {
        var (email, callbackURL) = sendResetPasswordEmail;

        await emailMessageService.SendResetPasswordLinkAsync(email, callbackURL, cancellationToken);

        logger.LogResetPasswordEmailSent(email);
    }

    public async Task ConsumeAsync(SendChangeEmail sendChangeEmail, CancellationToken cancellationToken)
    {
        var (email, callbackURL) = sendChangeEmail;

        await emailMessageService.SendEmailChangingLinkAsync(email, callbackURL, cancellationToken);

        logger.LogEmailChangingLinkSent(email);
    }
}
