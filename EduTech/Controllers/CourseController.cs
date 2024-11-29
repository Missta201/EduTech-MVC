using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult CreateCourse()
        {
            return View("CreateCourse");
        }
    }
}
