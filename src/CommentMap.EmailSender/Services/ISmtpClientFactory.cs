using MailKit.Net.Smtp;

namespace CommentMap.EmailSender.Services;

public interface ISmtpClientFactory
{
    ISmtpClient CreateClient();
}
