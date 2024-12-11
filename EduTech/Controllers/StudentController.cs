using EduTech.Models;
using EduTech.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUserService userService;

        public StudentController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<ApplicationUser> users = await userService.GetUsersAsync();
            return View();
        }
    }
}
