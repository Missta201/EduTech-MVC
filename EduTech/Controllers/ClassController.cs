using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class ClassController : Controller
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
