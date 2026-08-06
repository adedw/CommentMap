namespace CommentMap.Application.Models;

public record CommentCardDto(
    Guid Id,
    double Longitude,
    double Latitude,
    string Title,
    string Text,
    DateTime CreatedAt);
