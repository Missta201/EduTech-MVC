using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class LecturerPageController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
