namespace CommentMap.EmailSender.Services;

public interface IMessageSenderService
{
    Task SendConfirmationLinkAsync(string email, string callbackURL, CancellationToken ct = default);
    Task SendEmailChangingLinkAsync(string email, string callbackURL, CancellationToken ct = default);
    Task SendResetPasswordLinkAsync(string email, string callbackURL, CancellationToken ct = default);
}