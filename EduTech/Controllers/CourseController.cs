using EduTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTech.Controllers
{
    public class CourseController : Controller
    {

        private readonly EduTechDbContext dbContext;

        public CourseController(EduTechDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Main()
        {

            ViewData["Title"] = "Main";
            ViewData["HideFooter"] = true;
            ViewData["HideHeader"] = true;
            var courses = await dbContext.Courses.AsNoTracking().ToListAsync();
            return View("Main", courses);
        }

        [HttpGet]
        public IActionResult Add()
        {

            ViewData["Title"] = "Thêm khóa học";
            ViewData["HideFooter"] = true;
            ViewData["HideHeader"] = true;
            return View("Add");
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCourseViewModel viewModel)
        {
            var course = new Course
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
            };
            await dbContext.Courses.AddAsync(course);
            await dbContext.SaveChangesAsync();


            ViewData["Title"] = "Thêm khóa học";
            ViewData["HideFooter"] = true;
            ViewData["HideHeader"] = true;

            return RedirectToAction("Main", "Course");
        }

        [HttpGet]
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

            ViewData["Title"] = "Thêm khóa học";
            ViewData["HideFooter"] = true;
            ViewData["HideHeader"] = true;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddCourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var course = await dbContext.Courses.FindAsync(viewModel.Id);
            if (course != null)
            {
                course.Name = viewModel.Name;
                course.Description = viewModel.Description;

                await dbContext.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction("Main", "Course");
        }

        [HttpPost]
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

            return RedirectToAction("Main", "Course");
        }


    }
}
