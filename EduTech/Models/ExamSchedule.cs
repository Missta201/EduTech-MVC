using System.ComponentModel.DataAnnotations;

namespace EduTech.Models;

public class ExamSchedule
{
    [Key]
    public int Id { get; set; }
    public int ClassId { get; set; }
    public Class? Class { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public AssignmentType AssignmentType { get; set; }
    public DateOnly ExamDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}