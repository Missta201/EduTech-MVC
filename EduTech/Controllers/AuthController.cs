using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View("Login");
        }

        public IActionResult Register()
        {
            return View("Register");
        }
    }
}
