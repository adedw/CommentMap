namespace CommentMap.EmailSender.Templates;

public interface IEmailMessageViewModel
{
    string Title { get; }
    string Subtitle { get; }
    string Explanation { get; }
    string CallbackURL { get; }
    string ButtonText { get; }
}