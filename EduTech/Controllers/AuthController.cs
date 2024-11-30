using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            // Ẩn footer
            ViewData["Title"] = "Login Page";
            ViewData["HideFooter"] = true;
            return View("Login");
        }

        public IActionResult Register()
        {
            ViewData["Title"] = "Register Page";
            ViewData["HideFooter"] = true;
            return View("Register");
        }
    }
}
