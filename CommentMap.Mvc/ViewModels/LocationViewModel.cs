using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CommentMap.Mvc.ViewModels;

public class LocationViewModel
{
    [ValidateNever]
    public double? Longitude { get; set; }

    [ValidateNever]
    public double? Latitude { get; set; }

    public override string ToString()
    {
        return $"[{Longitude}, {Latitude}]";
    }
}
