namespace EduTech.Models.ViewModel
{
    public class GradeViewModel
    {
        public int ClassId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public AssignmentType AssignmentType { get; set; }
        public double Score { get; set; }
        public string? Comments { get; set; }
    }
}
