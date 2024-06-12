using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    [ValidateNever]
    public double? Latitude { get; init; }

    [JsonPropertyName("longitude")]
    [ValidateNever]
    public double? Longitude { get; init; }
}
