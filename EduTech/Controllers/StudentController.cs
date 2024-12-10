using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Main()
        {
            ViewData["Title"] = "Danh sách học viên";
            return View("Main");
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewData["Title"] = "Thêm học viên";
            return View("Add");
        }

        [HttpGet]
        public IActionResult Edit()
        {
            ViewData["Title"] = "Sửa học viên";
            return View("Edit");
        }
    }
}
