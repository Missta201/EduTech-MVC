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
    public class LecturerController : Controller
    {
        private readonly EduTechDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LecturerController(EduTechDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }



        [HttpGet]
        [Authorize(Policy = "CanViewStudentsLectures")]
        public async Task<IActionResult> Index()
        {
            var lectures = await _context.Users
                .Join(_context.UserClaims,
                    user => user.Id,
                    claim => claim.UserId,
                    (user, claim) => new { User = user, Claim = claim })
                .Where(x => x.Claim.ClaimType == "UserType" && x.Claim.ClaimValue == UserTypes.Lecturer)
                .Select(x => x.User)
                .AsNoTracking()
                .ToListAsync();

            return View("Index", lectures);
        }

        [HttpGet]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public IActionResult Add()
        {
            return View(new LecturerViewModel());
        }

        [HttpPost]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public async Task<IActionResult> Add(LecturerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = UserTypes.Lecturer,
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
                    await _userManager.AddClaimAsync(user, new Claim("UserType", UserTypes.Lecturer));
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
            var lecturer = await _userManager.FindByIdAsync(id);
            if (lecturer == null)
            {
                return NotFound();
            }

            var viewModel = new LecturerViewModel
            {
                Id = lecturer.Id,
                Name = lecturer.Name ?? string.Empty,
                Email = lecturer.Email ?? string.Empty,
                PhoneNumber = lecturer.PhoneNumber ?? string.Empty
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public async Task<IActionResult> Edit(LecturerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Id == null)
            {
                return NotFound();
            }

            // Update existing lecturer
            var existingLecturer = await _userManager.FindByIdAsync(model.Id);
            if (existingLecturer == null)
            {
                return NotFound();
            }
            var lecturer = existingLecturer;

            lecturer.Name = model.Name;
            lecturer.Email = model.Email;
            lecturer.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(lecturer);
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
            var lecturer = await _userManager.FindByIdAsync(id);
            if (lecturer == null)
            {
                return NotFound();
            }
            _context.Users.Remove(lecturer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
