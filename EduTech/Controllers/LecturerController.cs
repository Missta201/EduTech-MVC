using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class LecturerController : Controller
    {
        public IActionResult Main()
        {
            ViewData["Title"] = "Danh sách giảng viên";
            return View("Main");
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewData["Title"] = "Thêm giảng viên";
            return View("Add");
        }

        [HttpGet]
        public IActionResult Edit()
        {
            ViewData["Title"] = "Sửa giảng viên";
            return View("Edit");
        }
    }
}
