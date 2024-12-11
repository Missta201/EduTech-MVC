using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
