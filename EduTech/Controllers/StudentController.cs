using EduTech.Models;
using EduTech.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduTech.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly EduTechDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentController(EduTechDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        [Authorize(Policy = "CanViewStudentsLectures")]
        public async Task<IActionResult> Index()
        {
            var students = await _context.Users
                .Join(_context.UserClaims,
                    user => user.Id,
                    claim => claim.UserId,
                    (user, claim) => new { User = user, Claim = claim })
                .Where(x => x.Claim.ClaimType == "UserType" && x.Claim.ClaimValue == UserTypes.Student)
                .Select(x => x.User)
                .AsNoTracking()
                .ToListAsync();

            return View("Index", students);
        }

        [HttpGet]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public IActionResult Add()
        {
            return View(new StudentViewModel());
        }

        [HttpPost]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public async Task<IActionResult> Add(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = UserTypes.Student,
                    // Tạm thời set EmailConfirmed = true để không cần xác nhận email
                    EmailConfirmed = true

                };
                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password is required.");
                    return View(model);
                }
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddClaimAsync(user, new Claim("UserType", UserTypes.Student));
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public async Task<IActionResult> Edit(string id)
        {
            var student = await _userManager.FindByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name ?? string.Empty,
                Email = student.Email ?? string.Empty,
                PhoneNumber = student.PhoneNumber ?? string.Empty
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public async Task<IActionResult> Edit(StudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Id == null)
            {
                return NotFound();
            }

            // Update existing student
            var existingStudent = await _userManager.FindByIdAsync(model.Id);
            if (existingStudent == null)
            {
                return NotFound();
            }
            var student = existingStudent;

            student.Name = model.Name;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(student);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Policy = "CanDeleteStudentsLectures")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var student = await _userManager.FindByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            _context.Users.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}