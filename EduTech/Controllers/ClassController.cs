using EduTech.Models;
using EduTech.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduTech.Controllers
{
    public class ClassController : Controller
    {
        private readonly EduTechDbContext _context;

        public ClassController(EduTechDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes
                .Include(c => c.Course)
                .ToListAsync();
            return View("Index", classes);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var viewModel = new ClassViewModel {
                Courses = _context.Courses
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()

            };
            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ClassViewModel viewModel)
        {
            if (ModelState.IsValid) {
                var newClass = new Class
                {
                    Name = viewModel.Name,
                    RoomNumber = viewModel.RoomNumber,
                    Capacity = viewModel.Capacity,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    // Course
                    CourseId = viewModel.CourseId,
                    Course = null
                };
                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            // Repopulate courses dropdown if model is invalid
            viewModel.Courses = _context.Courses
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            return View("Edit",viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) 
        {
            var selectedClass = await _context.Classes
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (selectedClass == null)
            {
                return NotFound();
            }

            var viewModel = new ClassViewModel
            {
                Id = selectedClass.Id,
                Name = selectedClass.Name,
                RoomNumber = selectedClass.RoomNumber,
                Capacity = selectedClass.Capacity,
                StartDate = selectedClass.StartDate,
                EndDate = selectedClass.EndDate,
                CourseId = selectedClass.CourseId,
                Courses = _context.Courses
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList()
            };
            return View("Edit",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClassViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var selectedClass = await _context.Classes
                    .Include(c => c.Course)
                    .FirstOrDefaultAsync(c => c.Id == viewModel.Id);

                if (selectedClass == null)
                {
                    return NotFound();
                }

                selectedClass.Name = viewModel.Name;
                selectedClass.RoomNumber = viewModel.RoomNumber;
                selectedClass.Capacity = viewModel.Capacity;
                selectedClass.StartDate = viewModel.StartDate;
                selectedClass.EndDate = viewModel.EndDate;
                selectedClass.CourseId = viewModel.CourseId;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            // Repopulate courses dropdown if model is invalid
            viewModel.Courses = _context.Courses
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var selectedClass = await _context.Classes.FindAsync(id);
            if (selectedClass == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(selectedClass);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
