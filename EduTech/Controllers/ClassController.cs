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
        private readonly IAuthorizationService _authorizationService;
        public ClassController(EduTechDbContext context, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            this._authorizationService = authorizationService;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // Nếu là giảng thì chỉ xem được các lớp học đang chờ để đăng ký dạy
            if (User != null && (await _authorizationService.AuthorizeAsync(User, "IsLecturer")).Succeeded)
            {
                var classes = await _context.Classes
                    .Include(c => c.Course)
                    .Include(c => c.ClassSchedules)
                    .Include(c => c.Lecturers)
                    .Where(c => c.Status == ClassStatus.Pending)
                    .AsNoTracking()
                    .ToListAsync();
                return View("Index", classes);
            }
            // Nếu là học viên hay người dùng chưa đăng nhập thì chỉ xem được các lớp học đang mở
            else if (User?.Identity?.IsAuthenticated != true || (await _authorizationService.AuthorizeAsync(User, "IsStudent")).Succeeded)
            {
                var classes = await _context.Classes
                    .Include(c => c.Course)
                    .Include(c => c.ClassSchedules)
                    .Include(c => c.Lecturers)
                    .Include(c => c.Students)
                    .Where(c => c.Status == ClassStatus.Open)
                    .AsNoTracking()
                    .ToListAsync();
                return View("Index", classes);
            }
            else
            {
                // Define the priority order for ClassStatus
                var statusOrder = new[]
                {
                    ClassStatus.Pending,
                    ClassStatus.Open,
                    ClassStatus.InProgress,
                    ClassStatus.PaymentPending,
                    ClassStatus.Archived
                };

                // Nếu là giáo vụ hoặc admin thì xem được tất cả các lớp học và sắp xếp theo trạng thái
                var classes = await _context.Classes
                    .Include(c => c.Course)
                    .Include(c => c.ClassSchedules)
                    .Include(c => c.Lecturers)
                    .Include(c => c.Students)
                    .AsNoTracking()
                    .ToListAsync();

                // Sort classes by status order
                classes = classes.OrderBy(c => Array.IndexOf(statusOrder, c.Status)).ToList();

                return View("Index", classes);
            }
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

        // Giảng viên đăng ký dạy lớp học
        [HttpPost]
        [Authorize(Policy = "IsLecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterToTeach(int classId)
        {
            // Retrieve the class
            var classToTeach = await _context.Classes
                .Include(c => c.Lecturers)
                .Include(c => c.ClassSchedules)
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
                TempData["ErrorMessage"] = "Bạn đã đăng ký dạy lớp học này";
                return RedirectToAction("Index");
            }

            // Nếu trạng thái của lớp học không phải là Pending thì không thể đăng ký dạy
            if (classToTeach.Status != ClassStatus.Pending)
            {
                TempData["ErrorMessage"] = "Không thể đăng ký dạy lớp học đã mở";
                return RedirectToAction("Index");
            }

            // Kiểm tra trùng lịch
            // Retrieve all the classes the lecturer is already teaching, including their schedules
            var teachingClasses = await _context.Classes
                .Include(c => c.ClassSchedules)
                .Where(c => c.Lecturers.Any(l => l.Id == lecturer.Id))
                .ToListAsync();

            foreach (var teachingClass in teachingClasses)
            {
                // Skip if the date ranges do not overlap
                if (classToTeach.EndDate < teachingClass.StartDate || classToTeach.StartDate > teachingClass.EndDate)
                {
                    continue;
                }

                foreach (var existingSchedule in teachingClass.ClassSchedules)
                {
                    foreach (var newSchedule in classToTeach.ClassSchedules)
                    {
                        if (existingSchedule.Day == newSchedule.Day)
                        {
                            // Check if time overlaps
                            if (newSchedule.StartTime < existingSchedule.EndTime &&
                                existingSchedule.StartTime < newSchedule.EndTime)
                            {
                                TempData["ErrorMessage"] = $"Lịch dạy bị trùng với lớp {teachingClass.Name}";
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
            }
            // Assign lecturer to the class
            classToTeach.Lecturers.Add(lecturer);

            TempData["SuccessMessage"] = "Đăng ký lớp học thành công";

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Học viên đăng ký học lớp học
        [HttpPost]
        [Authorize(Policy = "IsStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int classId)
        {
            // Retrieve the class including its Students
            var classToEnroll = await _context.Classes
                .Include(c => c.Students)
                .Include(c => c.ClassSchedules)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (classToEnroll == null)
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
            if (classToEnroll.Students.Any(s => s.Id == student.Id))
            {
                TempData["ErrorMessage"] = "Bạn đã đăng ký học lớp này";
                return RedirectToAction("Index");
            }

            // Check if the class is full
            if (classToEnroll.NumberOfStudents >= classToEnroll.Capacity)
            {
                TempData["ErrorMessage"] = "Lớp học này đã đủ sĩ số";
                return RedirectToAction("Index");
            }

            // Nếu trạng thái của lớp học không phải là Open thì không thể đăng ký học
            if (classToEnroll.Status != ClassStatus.Open)
            {
                TempData["ErrorMessage"] = "Không thể đăng ký học lớp học đã tiến hành";
                return RedirectToAction("Index");
            }

            // Kiểm tra trùng lịch
            // Retrieve all the classes the lecturer is already teaching, including their schedules
            var enrollClasses = await _context.Classes
                .Include(c => c.ClassSchedules)
                .Where(c => c.Students.Any(s => s.Id == student.Id))
                .ToListAsync();

            foreach (var enrollClass in enrollClasses)
            {
                // Skip if the date ranges do not overlap
                if (classToEnroll.EndDate < enrollClass.StartDate || classToEnroll.StartDate > enrollClass.EndDate)
                {
                    continue;
                }

                foreach (var existingSchedule in enrollClass.ClassSchedules)
                {
                    foreach (var newSchedule in classToEnroll.ClassSchedules)
                    {
                        if (existingSchedule.Day == newSchedule.Day)
                        {
                            // Check if time overlaps
                            if (newSchedule.StartTime < existingSchedule.EndTime &&
                                existingSchedule.StartTime < newSchedule.EndTime)
                            {
                                TempData["ErrorMessage"] = $"Lịch học bị trùng với lớp {enrollClass.Name}";
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
            }

            // Enroll the student
            classToEnroll.Students.Add(student);
            // Increase student numbers 
            classToEnroll.NumberOfStudents++;

            TempData["SuccessMessage"] = "Đăng ký lớp học thành công";

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Giáo vụ hoặc admin thay đổi trạng khái của lớp học
        [HttpPost]
        [Authorize(Policy = "CanManageClasses")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, ClassStatus newStatus)
        {
            var classToUpdate = await _context.Classes
                .Include(c => c.Lecturers)
                .Include(s => s.Students)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classToUpdate == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu lớp học chưa có giảng viên nào đăng ký dạy thì không thể mở lớp
            if (newStatus == ClassStatus.Open && classToUpdate.Lecturers.Count == 0)
            {
                TempData["ErrorMessage"] = "Không thể mở lớp học khi chưa có giảng viên nào đăng ký dạy";
                return RedirectToAction("Index");
            }

            // Kiểm tra nếu lớp học chưa có sinh viên nào đăng ký học thì không thể tiến hành lớp
            if (newStatus == ClassStatus.InProgress && classToUpdate.Students.Count == 0)
            {
                TempData["ErrorMessage"] = "Không thể tiến hành học khi chưa có sinh viên nào đăng ký học";
                return RedirectToAction("Index");
            }

            classToUpdate.Status = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Giảng viên hủy đăng ký dạy lớp học
        [HttpPost]
        [Authorize(Policy = "IsLecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelTeaching(int classId)
        {
            var classToCancel = await _context.Classes
                .Include(c => c.Lecturers)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (classToCancel == null)
            {
                return NotFound();
            }

            var lecturer = await _userManager.GetUserAsync(User);
            if (lecturer == null)
            {
                return Unauthorized();
            }

            var lecturerToRemove = classToCancel.Lecturers.FirstOrDefault(l => l.Id == lecturer.Id);
            if (lecturerToRemove != null)
            {
                // Nếu lớp học đã mở thì không thể hủy đăng ký dạy
                if (classToCancel.Status != ClassStatus.Pending)
                {
                    TempData["ErrorMessage"] = "Không thể hủy đăng ký dạy lớp học đã mở";
                    return RedirectToAction("Index");
                }

                classToCancel.Lecturers.Remove(lecturerToRemove);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã hủy đăng ký dạy lớp học thành công";
            }

            return RedirectToAction("Index");
        }

        // Học viên hủy đăng ký học lớp
        [HttpPost]
        [Authorize(Policy = "IsStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelEnrollment(int classId)
        {
            var classToCancel = await _context.Classes
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (classToCancel == null)
            {
                return NotFound();
            }

            var student = await _userManager.GetUserAsync(User);
            if (student == null)
            {
                return Unauthorized();
            }

            // Nếu lớp học đã tiến hành thì không thể hủy đăng ký học
            if (classToCancel.Status != ClassStatus.Open)
            {
                TempData["ErrorMessage"] = "Không thể hủy đăng ký học lớp học đã tiến hành";
                return RedirectToAction("Index");
            }

            var studentToRemove = classToCancel.Students.FirstOrDefault(s => s.Id == student.Id);
            if (studentToRemove != null)
            {
                classToCancel.Students.Remove(studentToRemove);
                classToCancel.NumberOfStudents--;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã hủy đăng ký học lớp thành công";
            }

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
            var selectedClass = await _context.Classes
                .Include(c => c.ClassSchedules)  // Include related schedules
                .FirstOrDefaultAsync(c => c.Id == id);

            if (selectedClass == null)
            {
                return NotFound();
            }

            // Remove related schedules first
            if (selectedClass.ClassSchedules != null)
            {
                _context.ClassSchedules.RemoveRange(selectedClass.ClassSchedules);
            }

            // Then remove the class
            _context.Classes.Remove(selectedClass);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Danh sách lớp trong một lớp học
        [HttpGet]
        public async Task<IActionResult> ClassList(int id)
        {
            var selectedClass = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Lecturers)
                .Include(c => c.Students)
                .Include(c => c.StudentGrades)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (selectedClass == null)
            {
                return NotFound();
            }

            return View(selectedClass);
        }
        
        // Form gộp lớp
        [HttpGet]
        [Authorize(Policy = "CanManageClasses")]
        public IActionResult Merge()
        {
            var viewModel = new MergeClassesViewModel
            {
                Classes = _context.Classes
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList()
            };
            return View(viewModel);
        }
        
        // Gộp lớp, ghép lớp
        [HttpPost]
        [Authorize(Policy = "CanManageClasses")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MergeClasses(int classAId, int classBId)
        {
            var classA = await _context.Classes
                .Include(c => c.Students)
                .Include(c => c.Lecturers)
                .FirstOrDefaultAsync(c => c.Id == classAId);

            var classB = await _context.Classes
                .Include(c => c.Students)
                .Include(c => c.Lecturers)
                .FirstOrDefaultAsync(c => c.Id == classBId);

            if (classA == null || classB == null)
            {
                return NotFound();
            }

            // Transfer students from class B to class A
            foreach (var student in classB.Students)
            {
                if (!classA.Students.Any(s => s.Id == student.Id))
                {
                    classA.Students.Add(student);
                    classB.Students.Remove(student);
                }
            }

            // Transfer lecturers from class B to class A
            foreach (var lecturer in classB.Lecturers)
            {
                if (!classA.Lecturers.Any(l => l.Id == lecturer.Id))
                {
                    classA.Lecturers.Add(lecturer);
                    classB.Lecturers.Remove(lecturer);
                }
            }

            // Update the number of students in class A
            classA.NumberOfStudents = classA.Students.Count;
            classB.NumberOfStudents = classB.Students.Count;
            

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Gép lớp thành công";
            return RedirectToAction("Index");
        }


    }
}
