using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class DashboardController : Controller
    {
        [Route("dashboard")]
        public IActionResult Page()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["HideFooter"] = true;
            ViewData["HideHeader"] = true;
            return View("Page");
        }
    }
}
