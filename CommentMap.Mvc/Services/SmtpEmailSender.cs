using CommentMap.Mvc.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Text;

namespace CommentMap.Mvc.Services;

public class SmtpEmailSender(IOptions<SmtpEmailSenderOptions> options) : IEmailSender
{
    private readonly SmtpEmailSenderOptions _options = options.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using var mail = new MailMessage();
        mail.From = new MailAddress(_options.FromAddress);
        mail.To.Add(email);

        mail.SubjectEncoding = Encoding.UTF8;
        mail.Subject = subject;
        mail.BodyEncoding = Encoding.UTF8;
        mail.Body = htmlMessage;
        mail.IsBodyHtml = true;

        using var smtpClient = new SmtpClient(_options.Host, _options.Port);

        await smtpClient.SendMailAsync(mail);
    }
}
