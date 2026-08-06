namespace CommentMap.EmailSender.Services;

public interface ISmtpEmailSender
{
    Task SendHtmlEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}