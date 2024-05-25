namespace CommentMap.Mvc.ViewModels;

public record CommentCardViewModel(
    Guid Id,
    Coordinates Location,
    string Title,
    string Text,
    DateTime CreatedAt);

public record Coordinates(decimal Latitude, decimal Longitude);
