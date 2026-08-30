using MailKit.Security;

namespace CommentMap.EmailSender.Options;

public class SmtpSettings
{
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? FromName { get; set; }
    public string? FromAddress { get; set; }
    public SecureSocketOptions Security { get; set; } = SecureSocketOptions.None;
}
