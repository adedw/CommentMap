using System.ComponentModel.DataAnnotations;

namespace CommentMap.EmailSender.Options;

public class MailpitServiceOptions
{
    public const string SectionName = "services:mailpit";

    [Required]
    public required string[] Smtp { get; init; }

    [Required]
    public required string FromName { get; init; }

    [Required]
    public required string FromAddress { get; init; }

    public void Deconstruct(out string fromName, out string fromAddress, out string[] smtp)
    {
        fromName = FromName;
        fromAddress = FromAddress;
        smtp = Smtp;
    }
}
