using System.Security.Claims;
using EduTech.Models;
using EduTech.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTech.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EduTechDbContext _context;

        public DashboardController(UserManager<ApplicationUser> userManager, EduTechDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

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
        public async Task<IActionResult> AdminDashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                // Get total number of classes
                TotalClasses = await _context.Classes.CountAsync(),

                // Get total number of courses
                TotalCourses = await _context.Courses.CountAsync(),

                // Get total number of lecturers by checking UserClaims using UserManager
                TotalLecturers = (await _userManager.GetUsersForClaimAsync(new Claim("UserType", UserTypes.Lecturer))).Count,

                // Get total number of students by checking UserClaims using UserManager
                TotalStudents = (await _userManager.GetUsersForClaimAsync(new Claim("UserType", UserTypes.Student))).Count
            };

            return View(viewModel);
        }

        [Authorize(Policy = "IsLecturer")]
        public async Task<IActionResult> LecturerDashboard()
        {
            ApplicationUser? lecturer = await _userManager.GetUserAsync(User);
            return View(lecturer);
        }

        [Authorize(Policy = "IsStudent")]
        public async Task<IActionResult> StudentDashboard()
        {
            ApplicationUser? student = await _userManager.GetUserAsync(User);
            return View(student);
        }

    }
}
