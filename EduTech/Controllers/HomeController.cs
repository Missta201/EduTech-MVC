using System.Diagnostics;
using EduTech.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return View("BadRequest");
                case 404:
                    return View("NotFound");
                case 403:
                    return View("Forbid");
                case 401:
                    return View("Unauthorized");
                default:
                    return View("Error");
            }
        }
    }
}
