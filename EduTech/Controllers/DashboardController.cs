using EduTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Dựa vào claim UserType để xác định loại người dùng và sau đó chuyển hướng đến trang dashboard tương ứng
            if (User.HasClaim("UserType", UserTypes.Admin) || User.HasClaim("UserType", UserTypes.Scheduler))
            {
                return RedirectToAction("AdminDashboard");
            }
            else if (User.HasClaim("UserType", UserTypes.Lecturer))
            {
                return RedirectToAction("LecturerDashboard");
            }
            else if (User.HasClaim("UserType", UserTypes.Student))
            {
                return RedirectToAction("StudentDashboard");
            }

            return NotFound();
        }

        [Authorize(Policy = "IsAdminOrScheduler")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [Authorize(Policy = "IsLecturer")]
        public IActionResult LecturerDashboard()
        {
            return View();
        }

        [Authorize(Policy = "IsStudent")]
        public IActionResult StudentDashboard()
        {
            return View();
        }

    }
}
