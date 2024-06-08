using System.Text.Json.Serialization;

namespace CommentMap.Mvc.ViewModels;

public record CommentCardViewModel(
    Guid Id,
    Location Location,
    string Title,
    string Text,
    DateTime CreatedAt);


public record Location
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; init; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; init; }
}
