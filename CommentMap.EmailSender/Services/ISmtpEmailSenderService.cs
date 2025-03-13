namespace CommentMap.EmailSender.Services;

public interface ISmtpEmailSenderService
{
    Task SendHtmlEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}