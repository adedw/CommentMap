using CommentMap.EmailSender.Options;

using MailKit.Security;

using Microsoft.Extensions.Options;

using MimeKit;

namespace CommentMap.EmailSender.Services;

public class SmtpEmailSender : ISmtpEmailSender
{
    private readonly ISmtpClientFactory _smtpClientFactory;
    private readonly MailboxAddress _fromAddress;
    private readonly string _host;
    private readonly int _port;
    private readonly SecureSocketOptions _security;

    public SmtpEmailSender(ISmtpClientFactory smtpClientFactory, IOptions<SmtpSettings> options)
    {
        var value = options.Value;
        if (string.IsNullOrEmpty(value.Host))
        {
            throw new ArgumentException("Host must be provided.", nameof(options));
        }
        if (!value.Port.HasValue || value.Port <= 0)
        {
            throw new ArgumentException("Port must be a positive integer.", nameof(options));
        }
        if (string.IsNullOrEmpty(value.FromAddress))
        {
            throw new ArgumentException("FromAddress must be provided.", nameof(options));
        }

        _smtpClientFactory = smtpClientFactory;
        _host = value.Host;
        _port = value.Port.Value;
        _security = value.Security;
        _fromAddress = new MailboxAddress(value.FromName, value.FromAddress);
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

        using var smtpClient = _smtpClientFactory.CreateClient();

        await smtpClient.ConnectAsync(_host, _port, _security, ct);
        await smtpClient.SendAsync(message, ct);
        await smtpClient.DisconnectAsync(quit: true, ct);
    }
}
