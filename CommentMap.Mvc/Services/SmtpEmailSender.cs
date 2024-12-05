using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Text;

namespace CommentMap.Mvc.Services;

public class SmtpEmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using var mail = new MailMessage();
        mail.From = new MailAddress("no-reply@commentmap.com");
        mail.To.Add(email);

        mail.SubjectEncoding = Encoding.UTF8;
        mail.Subject = "Confirmation link";
        mail.BodyEncoding = Encoding.UTF8;
        mail.Body = htmlMessage;
        mail.IsBodyHtml = true;

        using var smtpClient = new SmtpClient("127.0.0.1", 1025);

        await smtpClient.SendMailAsync(mail);
    }
}
