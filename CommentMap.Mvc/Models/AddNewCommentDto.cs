namespace CommentMap.Mvc.Models;

public record AddNewCommentDto(Guid UserId, string Title, string Text, double Longitude, double Latitude);
