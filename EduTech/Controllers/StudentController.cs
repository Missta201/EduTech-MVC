using EduTech.Models;
using EduTech.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTech.Controllers
{
    public class StudentController : Controller
    {
        private readonly EduTechDbContext _context;

        public StudentController(EduTechDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.Users.AsNoTracking().ToListAsync();
            return View("Index", students);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View("Add");
        }

        [HttpGet]
        public IActionResult Edit()
        {
            return View("Edit");
        }
    }
}
