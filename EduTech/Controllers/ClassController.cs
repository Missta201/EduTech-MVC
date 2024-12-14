using EduTech.Models;
using EduTech.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduTech.Controllers
{
    [Authorize]
    public class ClassController : Controller
    {
        private readonly EduTechDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClassController(EduTechDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.ClassSchedules)
                .Include(c => c.Lecturers)
                .ToListAsync();
            return View("Index", classes);
        }

        [HttpGet]
        [Authorize(Policy = "CanManageClasses")]
        public IActionResult Add()
        {
            var viewModel = new ClassViewModel
            {
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
        [Authorize(Policy = "CanManageClasses")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ClassViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newClass = new Class
                {
                    Name = viewModel.Name,
                    RoomNumber = viewModel.RoomNumber,
                    Capacity = viewModel.Capacity,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    Tuition = viewModel.Tuition,
                    // Course
                    CourseId = viewModel.CourseId,
                    Course = null, //Don't touch this line please
                    // ClassSchedules
                    ClassSchedules = viewModel.ClassSchedules.Select(s => new ClassSchedule
                    {
                        Day = s.Day,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime
                    }).ToList(),
                    // Khi thêm một class, ban đầu classstatus sẽ là Pending để đơi giảng viên đăng ký day lớp đó
                    Status = ClassStatus.Pending,
                    NumberOfStudents = 0

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

            return View("Edit", viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "IsLecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterToTeach(int classId)
        {
            // Retrieve the class
            var classToTeach = await _context.Classes
                .Include(c => c.Lecturers)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (classToTeach == null)
            {
                return NotFound();
            }

            // Get the current lecturer
            var lecturer = await _userManager.GetUserAsync(User);

            if (lecturer == null)
            {
                return Unauthorized();
            }

            // Check if the lecturer is already assigned to this class
            if (classToTeach.Lecturers.Any(l => l.Id == lecturer.Id))
            {
                ModelState.AddModelError(string.Empty, "You are already registered to teach this class.");
                return RedirectToAction("Index");
            }

            // Assign lecturer to the class
            classToTeach.Lecturers.Add(lecturer);

            // Update class status if it's pending
            if (classToTeach.Status == ClassStatus.Pending)
            {
                classToTeach.Status = ClassStatus.Open;
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = "IsStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int classId)
        {
            // Retrieve the class including its Students
            var selectedClass = await _context.Classes
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (selectedClass == null)
            {
                return NotFound();
            }

            // Get the current student
            var student = await _userManager.GetUserAsync(User);

            if (student == null)
            {
                return Unauthorized();
            }

            // Check if the student is already enrolled
            if (selectedClass.Students.Any(s => s.Id == student.Id))
            {
                ModelState.AddModelError(string.Empty, "You are already enrolled in this class.");
                return RedirectToAction("Index");
            }

            // Check if the class is full
            if (selectedClass.NumberOfStudents > selectedClass.Capacity)
            {
                ModelState.AddModelError(string.Empty, "This class is already full.");
                return RedirectToAction("Index");
            }

            // Enroll the student
            selectedClass.Students.Add(student);
            // Increase student numbers 
            selectedClass.NumberOfStudents++;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Policy = "CanManageClasses")]
        public async Task<IActionResult> Edit(int id)
        {
            var selectedClass = await _context.Classes
            .Include(c => c.Course)
            .Include(c => c.ClassSchedules)
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
                Tuition = selectedClass.Tuition,
                // Course
                CourseId = selectedClass.CourseId,
                Courses = _context.Courses
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList(),
                // ClassSchedules
                ClassSchedules = selectedClass.ClassSchedules.Select(s => new ClassScheduleViewModel
                {
                    Id = s.Id,
                    Day = s.Day,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToList()
            };
            return View("Edit", viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "CanManageClasses")]
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
                    .Include(c => c.ClassSchedules) // Include ClassSchedules
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
                selectedClass.Tuition = viewModel.Tuition;

                // Remove existing schedules
                _context.ClassSchedules.RemoveRange(selectedClass.ClassSchedules);

                // Add new schedules
                selectedClass.ClassSchedules = viewModel.ClassSchedules.Select(s => new ClassSchedule
                {
                    Day = s.Day,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToList();

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
        [Authorize(Policy = "CanManageClasses")]
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
