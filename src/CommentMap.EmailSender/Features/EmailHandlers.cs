using CommentMap.EmailSender.Logging;
using CommentMap.EmailSender.Services;
using CommentMap.EventBus.Abstractions;
using CommentMap.Shared.Messages;

namespace CommentMap.EmailSender.Features;

public sealed class SendConfirmEmailHandler(IMessageSenderService emailMessageService, ILogger<SendConfirmEmailHandler> logger)
    : IIntegrationEventHandler<SendConfirmEmail>
{
    public async Task Handle(SendConfirmEmail message, CancellationToken cancellationToken)
    {
        await emailMessageService.SendConfirmationLinkAsync(message.Email, message.CallbackURL, cancellationToken);
        logger.LogConfirmationEmailSent(message.Email);
    }
}

public sealed class SendResetPasswordEmailHandler(IMessageSenderService emailMessageService, ILogger<SendResetPasswordEmailHandler> logger)
    : IIntegrationEventHandler<SendResetPasswordEmail>
{
    public async Task Handle(SendResetPasswordEmail message, CancellationToken cancellationToken)
    {
        await emailMessageService.SendResetPasswordLinkAsync(message.Email, message.CallbackURL, cancellationToken);
        logger.LogResetPasswordEmailSent(message.Email);
    }
}

public sealed class SendChangeEmailHandler(IMessageSenderService emailMessageService, ILogger<SendChangeEmailHandler> logger)
    : IIntegrationEventHandler<SendChangeEmail>
{
    public async Task Handle(SendChangeEmail message, CancellationToken cancellationToken)
    {
        await emailMessageService.SendEmailChangingLinkAsync(message.Email, message.CallbackURL, cancellationToken);
        logger.LogEmailChangingLinkSent(message.Email);
    }
}
