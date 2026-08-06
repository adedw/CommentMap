namespace CommentMap.EmailSender.Templates;


public class ConfirmEmailViewModel : IEmailMessageViewModel
{
    public required string CallbackURL { get; init; }
    public string Title => "Please confirm your email address";
    public string Subtitle => "Thanks for signing up to CommentMap. We're happy to have you.";
    public string Explanation => "One more step, you need to confirm the email you provided during registration.";
    public string ButtonText => "Confirm email address";
}
