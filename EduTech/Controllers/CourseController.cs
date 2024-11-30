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

            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> List()
        //{
        //   var courses = await dbContext.Courses.ToListAsync();

        //    return View(courses);
        //}

        [HttpGet]
        public async Task<IActionResult> Edit(Guid Name)
        {
            var course = await dbContext.Courses.FindAsync(Name);
            return View(course);
        }

        //[HttpPost]
        //public async Task<IActionResult> Edit(Course viewModel)
        //{
        //    var course = await dbContext.Courses.FindAsync(viewModel.Name);
        //    if (course is not null)
        //    {
        //        course.Name = viewModel.Name;
        //        course.Description = viewModel.Description;

        //        await dbContext.SaveChangesAsync();
        //    }
        //    return RedirectToAction("List", "Course");
        //}
        [HttpPost]
        public async Task<IActionResult> Edit(Course viewModel)
        {
            var course = await dbContext.Courses.FindAsync(viewModel.Name); // Use the primary key
            if (course != null)
            {
                // Update fields
                course.Name = viewModel.Name;
                course.Description = viewModel.Description;

                await dbContext.SaveChangesAsync();
            }
            else
            {
                return NotFound(); 
            }

            return RedirectToAction("List", "Course");
        }


    }
}
