using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduTech.Models.ViewModel
{
    public class ClassViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string RoomNumber { get; set; } = String.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int Capacity { get; set; }
        public double Tuition { get; set; }

        public ClassStatus Status { get; set; }

        public int CourseId { get; set; }

        public List<SelectListItem> Courses { get; set; } = [];
        public List<ClassScheduleViewModel> ClassSchedules { get; set; } = [];

    }

    public class ClassScheduleViewModel
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
