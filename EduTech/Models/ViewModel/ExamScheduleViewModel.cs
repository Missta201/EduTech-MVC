using System.ComponentModel.DataAnnotations;

namespace EduTech.Models.ViewModel;

public class ExamScheduleViewModel : IValidatableObject
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;

    [Required]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    public AssignmentType AssignmentType { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateOnly ExamDate { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeOnly StartTime { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeOnly EndTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime > EndTime)
        {
            yield return new ValidationResult("Giờ bắt đầu không được lơn hơn giờ kết thúc", new[] { nameof(StartTime), nameof(EndTime) });
        }
    }
}