using CommentMap.Mvc.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace CommentMap.Mvc.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LocationRequiredAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var valueType = value.GetType();

        var latitudeProperty = valueType.GetProperty(nameof(LocationViewModel.Latitude));
        var longitudeProperty = valueType.GetProperty(nameof(LocationViewModel.Longitude));

        var longitudeValue = longitudeProperty.GetValue(value);
        var latitudeValue = latitudeProperty.GetValue(value);

        if (longitudeValue is double longitude
            && latitudeValue is double latitude
            && !IsEqual(longitude, 0)
            && !IsEqual(latitude, 0))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage);
    }

    private static bool IsEqual(double value1, double value2)
    {
        return Math.Abs(value1 - value2) < 0.00001;
    }
}
