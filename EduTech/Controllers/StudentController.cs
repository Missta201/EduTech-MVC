using EduTech.Models;
using EduTech.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class StudentController : Controller
    {


       

        public IActionResult Index()
        {
            return View("Index");
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
