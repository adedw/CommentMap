using MailKit.Net.Smtp;

namespace CommentMap.EmailSender.Services;

public class SmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClient CreateClient()
    {
        var client = new SmtpClient();
        return client;
    }
}