using CommentMap.EmailSender.Logging;
using CommentMap.Shared.Messages;
using MassTransit;

namespace CommentMap.EmailSender.Services;

internal class MessageSenderConsumer(
    IMessageSenderService emailMessageService,
    ILogger<MessageSenderConsumer> logger)
    : IConsumer<SendConfirmEmail>
    , IConsumer<SendResetPasswordEmail>
    , IConsumer<SendChangeEmail>
{
    public async Task Consume(ConsumeContext<SendConfirmEmail> context)
    {
        var (email, callbackURL) = context.Message;

        await emailMessageService.SendConfirmationLinkAsync(email, callbackURL, context.CancellationToken);

        logger.LogConfirmationEmailSent(email);
    }

    public async Task Consume(ConsumeContext<SendResetPasswordEmail> context)
    {
        var (email, callbackURL) = context.Message;
        
        await emailMessageService.SendResetPasswordLinkAsync(email, callbackURL, context.CancellationToken);

        logger.LogResetPasswordEmailSent(email);
    }

    public async Task Consume(ConsumeContext<SendChangeEmail> context)
    {
        var (email, callbackURL) = context.Message;

        await emailMessageService.SendEmailChangingLinkAsync(email, callbackURL, context.CancellationToken);

        logger.LogEmailChangingLinkSent(email);
    }
}
