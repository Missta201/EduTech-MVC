using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class ErrorCodeController : Controller
    {
        public new IActionResult NotFound()
        {
            return View("NotFound");
        }

        public IActionResult Forbidden()
        { 
            return View("Forbidden");
        }
    }
}
