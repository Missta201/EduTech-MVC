﻿using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class ClassController : Controller
    {
        public IActionResult Main()
        {
            ViewData["Title"] = "Danh sách lớp học";
            return View("Main");
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewData["Title"] = "Thêm lớp học";
            return View("Add");
        }

        [HttpGet]
        public IActionResult Edit() 
        {

            ViewData["Title"] = "Sửa lớp học";
            return View("Edit");
        }
    }
}