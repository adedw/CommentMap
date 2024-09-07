using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CommentMap.Mvc.ViewModels;

public class LocationViewModel
{
    [Required]
    [Range(-20037508.3427892, 20037508.3427892)]
    public double? Longitude { get; init; }

    [Required]
    [Range(-20037508.3427892, 20037508.3427892)]
    public double? Latitude { get; init; }

    public string GetJsonArray()
    {
        if (!Longitude.HasValue)
        {
            throw new NullReferenceException("Longitude has no value.");
        }
        if (!Latitude.HasValue)
        {
            throw new NullReferenceException("Latitude has no value.");
        }

        return $"[{Longitude.Value.ToString(CultureInfo.InvariantCulture)}, {Latitude.Value.ToString(CultureInfo.InvariantCulture)}]";
    }
}
