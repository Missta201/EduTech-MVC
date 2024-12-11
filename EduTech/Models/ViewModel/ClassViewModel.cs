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
        public int CourseId { get; set; }

        public List<SelectListItem> Courses { get; set; } = [];
    }
}
