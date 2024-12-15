using System.ComponentModel.DataAnnotations;

namespace EduTech.Models
{
    public class StudentGrade
    {
        [Key]
        public int Id { get; set; }
        public int ClassId { get; set; }
        public Class? Class { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }
        public AssignmentType AssignmentType { get; set; }
        public double Score { get; set; }
        public string? Comments { get; set; }
    }

    public enum AssignmentType
    {
        Midterm,
        Final
    }
}
