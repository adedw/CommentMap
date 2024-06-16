namespace CommentMap.Mvc.ViewModels;

public record CommentCardViewModel(
    Guid Id,
    LocationViewModel Location,
    string Title,
    string Text,
    DateTime CreatedAt);
