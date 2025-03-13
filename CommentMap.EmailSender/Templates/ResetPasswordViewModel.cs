namespace CommentMap.EmailSender.Templates;

public class ResetPasswordViewModel : IEmailMessageViewModel
{
    public required string CallbackURL { get; init; }
    public string Title => "Please confirm password reset";
    public string Subtitle => "Someone has requested a password change.";
    public string Explanation => "You must confirm the password reset. If this is not you, please ignore this message.";
    public string ButtonText => "Confirm reset password";

}
