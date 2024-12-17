using EduTech.Models;
using EduTech.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTech.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {
        private readonly EduTechDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ExamController(EduTechDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // Danh sách các lớp đang trong quá trình học để tạo lịch thi
        [HttpGet]
        [Authorize(Policy = "IsAdminOrScheduler")]
        public async Task<IActionResult> CurrentExamSchedule()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Fetch InProgress classes taught by the lecturer
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.ExamSchedules)
                .Where(c => c.Status == ClassStatus.InProgress )
                .ToListAsync();

            return View("CurrentExamSchedule", classes);
        }
        // Form tạo lịch thi cho lớp học
        [HttpGet]
        [Authorize(Policy = "IsAdminOrScheduler")]
        public async Task<IActionResult> Create(int classId)
        {
            var classEntity = await _context.Classes.FindAsync(classId);
            
            if (classEntity == null)
            {
                return NotFound();
            }

            var viewModel = new ExamScheduleViewModel
            {
                ClassId = classId,
                ClassName = classEntity.Name
            };

            return View("Edit",viewModel);
        }
        
        [HttpPost]
        [Authorize(Policy = "IsAdminOrScheduler")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamScheduleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if the ClassId exists in the Classes table
                var classExists = await _context.Classes.AnyAsync(c => c.Id == viewModel.ClassId);
                if (!classExists)
                {
                    TempData["ErrorMessage"]= "Lớp học không tồn tại";
                    return View("Edit",viewModel);
                }
                
                // Mỗi một lớp học chỉ được tạo một lịch thi cho mỗi loại bài kiểm tra
                //Check if AssignmentType exists
                var assignmentTypeExists = await _context.ExamSchedules
                    .AnyAsync(es => es.ClassId == viewModel.ClassId && es.AssignmentType == viewModel.AssignmentType);

                if (assignmentTypeExists)
                {
                    TempData["ErrorMessage"] = "Lịch thi cho loại bài kiểm tra này đã tồn tại.";
                    return View("Edit", viewModel);
                }
                
                // Kiểm tra nếu ngày thi vượt quá thời gian học của lớp 
                var classEntity = await _context.Classes.FindAsync(viewModel.ClassId);
                if (viewModel.ExamDate > classEntity.EndDate)
                {
                    TempData["ErrorMessage"] = "Ngày thi không được vượt quá thời gian học của lớp.";
                    return View("Edit", viewModel);
                }
                // Kiểm tra nếu ngày thi  ngày thi nhỏ hơn ngày bắt đầu học của lớp
                if (viewModel.ExamDate < classEntity.StartDate)
                {
                    TempData["ErrorMessage"] = "Ngày thi không được nhỏ hơn thời gian học của lớp.";
                    return View("Edit", viewModel);
                }

                var examSchedule = new ExamSchedule
                {
                    ClassId = viewModel.ClassId,
                    AssignmentType = viewModel.AssignmentType,
                    ExamDate = viewModel.ExamDate,
                    StartTime = viewModel.StartTime,
                    EndTime = viewModel.EndTime,
                    RoomNumber = viewModel.RoomNumber
                };

                _context.ExamSchedules.Add(examSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction("CurrentExamSchedule", "Exam");
            }

            return View("Edit",viewModel);
        }

        [HttpGet]
        [Authorize(Policy = "IsAdminOrScheduler")]
        public async Task<IActionResult> Edit(int id)
        {
            var examSchedule = await _context.ExamSchedules.FindAsync(id);
            if (examSchedule == null)
            {
                return NotFound();
            }

            var viewModel = new ExamScheduleViewModel
            {
                Id = examSchedule.Id,
                ClassId = examSchedule.ClassId,
                AssignmentType = examSchedule.AssignmentType,
                ExamDate = examSchedule.ExamDate,
                StartTime = examSchedule.StartTime,
                EndTime = examSchedule.EndTime,
                RoomNumber = examSchedule.RoomNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "IsAdminOrScheduler")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExamScheduleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var examSchedule = await _context.ExamSchedules.FindAsync(viewModel.Id);
                if (examSchedule == null)
                {
                    return NotFound();
                }

                examSchedule.AssignmentType = viewModel.AssignmentType;
                examSchedule.ExamDate = viewModel.ExamDate;
                examSchedule.StartTime = viewModel.StartTime;
                examSchedule.EndTime = viewModel.EndTime;
                examSchedule.RoomNumber = viewModel.RoomNumber;

                await _context.SaveChangesAsync();
                return RedirectToAction("CurrentExamSchedule","Exam");
            }

            return View(viewModel);
        }
        
        [HttpPost]
        [Authorize(Policy = "IsAdminOrScheduler")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var examSchedule = await _context.ExamSchedules.FindAsync(id);
            if (examSchedule == null)
            {
                return NotFound();
            }

            _context.ExamSchedules.Remove(examSchedule);
            await _context.SaveChangesAsync();
            return RedirectToAction("CurrentExamSchedule","Exam");
        }

        [AllowAnonymous]
        public IActionResult ExamResults()
        {
            ViewData["Title"] = "Tra cứu điểm / Kết quả học tập EduTECH";
            return View("ExamResults");
        }
    }
}
