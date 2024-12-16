using System.ComponentModel.DataAnnotations;

namespace EduTech.Attributes;

public class EndDateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var endDate = (DateOnly)value;
        if (endDate < DateOnly.FromDateTime(DateTime.Now))
        {
            return new ValidationResult("Ngày kết thúc không được bé hơn ngày hiện tại");
        }

        return ValidationResult.Success;
    }
}