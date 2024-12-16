using System.ComponentModel.DataAnnotations;

namespace EduTech.Attributes;

public class UniqueCourseNameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var dbContext = (EduTechDbContext)validationContext.GetService(typeof(EduTechDbContext));
        var courseName = value as string;
        var courseId = (int?)validationContext.ObjectType.GetProperty("Id")?.GetValue(validationContext.ObjectInstance, null);

        var course = dbContext.Courses
            .FirstOrDefault(c => c.Name == courseName && (!courseId.HasValue || c.Id != courseId.Value));

        if (course != null)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}