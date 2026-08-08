using System.Text.Json;

namespace CommentMap.Mvc.ViewModels;

public record CommentCardViewModel(
    Guid Id,
    double Longitude,
    double Latitude,
    string Title,
    string Text,
    DateTime CreatedAt)
{
    /// <summary>
    /// Coordinates as a JSON array in [longitude, latitude] order (EPSG:3857 values).
    /// </summary>
    public string CoordinatesJson => JsonSerializer.Serialize(new[] { Longitude, Latitude });
}
