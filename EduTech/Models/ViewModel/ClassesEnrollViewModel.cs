namespace EduTech.Models.ViewModel
{
    public class ClassesEnrollViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public List<ClassSchedule> Schedule { get; set; } = new List<ClassSchedule>();
        public ClassStatus Status { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }
}
