namespace CommentMap.Shared.Messages;

public record SendResetPasswordEmail(string Email, string CallbackURL);
