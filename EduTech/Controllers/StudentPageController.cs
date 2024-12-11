using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class StudentPageController : Controller
    {
        public IActionResult Index()
        {

            return View("Index");
        }

    }
}
