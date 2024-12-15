namespace EduTech.Models
{
    public class ScheduleData
    {
        public int Id { get; set; }
        public string Subject { get; set; } = String.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
