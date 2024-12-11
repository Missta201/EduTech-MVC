using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class ErrorCodeController : Controller
    {
        public IActionResult NotFound()
        {
            return View("NotFound");
        }

        public IActionResult Forbidden()
        { 
            return View("Forbidden");
        }
    }
}
