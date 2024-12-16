using System.ComponentModel.DataAnnotations;

namespace EduTech.Attributes;

public class StartDateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var startDate = (DateOnly)value;
        if (startDate < DateOnly.FromDateTime(DateTime.Now))
        {
            return new ValidationResult("Ngày bắt đầu không được bé hơn ngày hiện tại");
        }

        return ValidationResult.Success;
    }
}