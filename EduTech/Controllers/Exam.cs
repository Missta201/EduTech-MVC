﻿using Microsoft.AspNetCore.Mvc;

namespace EduTech.Controllers
{
    public class Exam : Controller
    {
        public IActionResult ExamResults()
        {
            ViewData["Title"] = "Tra cứu điểm / Kết quả học tập EduTECH";
            ViewData["HideFooter"] = true;
            return View("ExamResults");
        }
    }
}
