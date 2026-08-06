namespace CommentMap.EmailSender.Options;

public class MailpitClientSettings
{
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? FromName { get; set; }
    public string? FromAddress { get; set; }
}
