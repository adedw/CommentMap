using CommentMap.Mvc.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace CommentMap.Mvc.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LocationRequiredAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var valueType = value.GetType();

        var latitudeProperty = valueType.GetProperty(nameof(Location.Latitude));
        var longitudeProperty = valueType.GetProperty(nameof(Location.Longitude));

        var longitudeValue = longitudeProperty.GetValue(value);
        var latitudeValue = latitudeProperty.GetValue(value);

        if (longitudeValue is null || latitudeValue is null)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
