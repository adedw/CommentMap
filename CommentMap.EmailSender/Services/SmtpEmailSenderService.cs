using CommentMap.EmailSender.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CommentMap.EmailSender.Services;

public class SmtpEmailSenderService : ISmtpEmailSenderService
{
    private readonly ISmtpClient _smtpClient;
    private readonly MailboxAddress _fromAddress;
    private readonly string _host;
    private readonly int _port;
  
    public SmtpEmailSenderService(ISmtpClient smtpClient, IOptions<MailpitServiceOptions> options)
    {
        _smtpClient = smtpClient;

        var (fromName, fromAddress, smtp) = options.Value;
        _fromAddress = new MailboxAddress(fromName, fromAddress);

        var uri = new Uri(smtp[0]);
        _host = uri.Host;
        _port = uri.Port;
    }

   public async Task SendHtmlEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(_fromAddress);
        message.To.Add(new MailboxAddress(null, to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };

        message.Body = bodyBuilder.ToMessageBody();

        await _smtpClient.ConnectAsync(_host, _port, SecureSocketOptions.None, ct);

        await _smtpClient.SendAsync(message, ct);

        await _smtpClient.DisconnectAsync(quit: true, ct);
    }
}