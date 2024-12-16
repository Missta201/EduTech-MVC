using System.ComponentModel.DataAnnotations;

namespace EduTech.Attributes;

public class UniqueClassNameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var dbContext = (EduTechDbContext)validationContext.GetService(typeof(EduTechDbContext));
        var className = value as string;
        var classId = (int?)validationContext.ObjectType.GetProperty("Id")?.GetValue(validationContext.ObjectInstance, null);

        var classEntity = dbContext.Classes
            .FirstOrDefault(c => c.Name == className && (!classId.HasValue || c.Id != classId.Value));

        if (classEntity != null)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}