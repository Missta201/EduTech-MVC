using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class ClassController : Controller
    {
        [HttpGet]
        public IActionResult Add()
        {
            ViewData["Title"] = "Thêm lớp học";
            ViewData["HideFooter"] = true;
            ViewData["HideHeader"] = true;
            return View("Add");
        }
    }
}
