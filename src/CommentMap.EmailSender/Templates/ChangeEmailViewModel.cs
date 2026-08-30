namespace CommentMap.EmailSender.Templates;

public class ChangeEmailViewModel : IEmailMessageViewModel
{
    public required string CallbackURL { get; init; }
    public string Title => "Please confirm the change of email address";
    public string Subtitle => "To complete changing email you must confirm new one.";
    public string Explanation => "To approve a new email address, we need to verify that you are the owner of it.";
    public string ButtonText => "Confirm email address";

}
