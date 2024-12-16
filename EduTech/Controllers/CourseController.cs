using EduTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTech.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {

        private readonly EduTechDbContext dbContext;

        public CourseController(EduTechDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var courses = await dbContext.Courses.AsNoTracking().ToListAsync();
            return View("Index", courses);
            
        }

        [HttpGet]
        [Authorize (Policy = "CanManageCourses")]
        public IActionResult Add()
        {
            return View("Add");
        }

        [HttpPost]
        [Authorize (Policy = "CanManageCourses")]
        public async Task<IActionResult> Add(AddCourseViewModel viewModel)
        {
            // Kiểm tra xem dữ liệu có hợp lệ không
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var course = new Course
            {
                Name = viewModel.Name, 
                Description = viewModel.Description
            };

            await dbContext.Courses.AddAsync(course);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Course");
        }

        [HttpGet]
        [Authorize(Policy = "CanManageCourses")]

        public async Task<IActionResult> Edit(int id)
        {
            var course = await dbContext.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            // Tạo view model từ entity để gửi dữ liệu sang view
            var viewModel = new AddCourseViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize(Policy = "CanManageCourses")]

        public async Task<IActionResult> Edit(AddCourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var course = await dbContext.Courses.FindAsync(viewModel.Id);
            if (course != null)
            {
                course.Name = viewModel.Name ?? string.Empty; 
                course.Description = viewModel.Description;

                await dbContext.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Course");
        }

        

        [HttpPost]
        [Authorize(Policy = "IsAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await dbContext.Courses.FindAsync(id);

            if (course != null)
            {
                dbContext.Courses.Remove(course);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Course");
        }


    }
}
