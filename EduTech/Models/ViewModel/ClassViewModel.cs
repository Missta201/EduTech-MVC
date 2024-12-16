using System.ComponentModel.DataAnnotations;
using EduTech.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduTech.Models.ViewModel
{
    public class ClassViewModel : IValidatableObject
    {
        public int Id { get; set; }
        [Required]
        [UniqueClassName]
        public string Name { get; set; } = String.Empty;
        public string RoomNumber { get; set; } = String.Empty;
        [Required]
        [DataType(DataType.Date)]
        [StartDateValidation]
        public DateOnly StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [EndDateValidation]
        public DateOnly EndDate { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa lớp phải lớn hơn 0")]
        public int Capacity { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Học phí phải lớn hơn 0")]
        public double Tuition { get; set; }

        public ClassStatus Status { get; set; }

        [Required]
        public int CourseId { get; set; }

        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>();
        public List<ClassScheduleViewModel> ClassSchedules { get; set; } = new List<ClassScheduleViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                yield return new ValidationResult("Ngày bắt đầu không thể lớn hơn ngày kết thúc", new[] { nameof(StartDate), nameof(EndDate) });
            }

            var scheduleDays = new HashSet<DayOfWeek>();

            foreach (var schedule in ClassSchedules)
            {
                if (!scheduleDays.Add(schedule.Day))
                {
                    yield return new ValidationResult("Lịch học không được trùng ngày", new[] { nameof(ClassSchedules) });
                }
            }
        }
    }

    public class ClassScheduleViewModel : IValidatableObject
    {
        public int Id { get; set; }
        [Required]
        public DayOfWeek Day { get; set; }
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime > EndTime)
            {
                yield return new ValidationResult("Giờ bắt đầu không được lớn hơn giờ kết thúc", new[] { nameof(StartTime), nameof(EndTime) });
            }
        }
    }
}
