using System.ComponentModel.DataAnnotations;

namespace CommentMap.Mvc.Models;

public class SmtpEmailSenderOptions
{
    public string SmtpEmailSender => nameof(SmtpEmailSender);

    [Required]
    public required string FromAddress { get; init; }

    [Required]
    public required string Host { get; init; }

    [Required]
    public required short Port { get; init; }
}
