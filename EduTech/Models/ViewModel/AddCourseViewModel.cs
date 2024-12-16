using System.ComponentModel.DataAnnotations;
using EduTech.Attributes;

namespace EduTech.Models
{
    public class AddCourseViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Tên khóa học")]
        [Required(ErrorMessage = "{0} là bắt buộc")]
        [UniqueCourseName(ErrorMessage = "Tên khoá học phải là duy nhất.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }


}
