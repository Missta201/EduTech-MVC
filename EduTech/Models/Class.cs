using System.ComponentModel.DataAnnotations;

namespace EduTech.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int Capacity { get; set; }
        //Học phí
        public double Tuition { get; set; }
        //Số lượng học viên đã đăng ký
        public int NumberOfStudents { get; set; }

        //Trạng thái của lớp học
        public ClassStatus Status { get; set; }

        //Một lớp học mở cho một môn học
        public int CourseId { get; set; }
        public Course Course { get; set; } = new Course();

        //Một lớp học có nhiều lịch học
        public List<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();
        // Một lớp học có nhiều lịch kiểm tra
        public List<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();

        // Một lớp học được dạy bởi nhiều giảng viên
        public List<ApplicationUser> Lecturers { get; set; } = new List<ApplicationUser>();
        // Một lớp học có nhiều sinh viên
        public List<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();
        // Danh sách điểm của sinh viên trong lớp học
        public List<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();

        // Danh sách hóa đơn của lớp học
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
