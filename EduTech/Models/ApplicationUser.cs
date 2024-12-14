using Microsoft.AspNetCore.Identity;

namespace EduTech.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? Name { get; set; }

        [PersonalData]
        public string? UserType { get; set; }

         // Danh sách các lớp học mà giảng viên đó dạy
        public List<Class> ClassesTeaching { get; set; } = new List<Class>();
        // Danh sách các lớp học mà học viên đó tham gia
        public List<Class> ClassesAttending { get; set; } = new List<Class>();
    }
}
