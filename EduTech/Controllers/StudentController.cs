using EduTech.Models;
using EduTech.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EduTech.Services;

namespace EduTech.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly EduTechDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PdfConverter _pdfConverter;

        public StudentController(EduTechDbContext context, UserManager<ApplicationUser> userManager, PdfConverter pdfConverter)
        {
            _context = context;
            _userManager = userManager;
            _pdfConverter = pdfConverter;
        }

        // Hiển thị danh sách học viên
        [HttpGet]
        [Authorize(Policy = "CanViewStudentsLectures")]
        public async Task<IActionResult> Index()
        {
            var students = await _context.Users
                .Join(_context.UserClaims,
                    user => user.Id,
                    claim => claim.UserId,
                    (user, claim) => new { User = user, Claim = claim })
                .Where(x => x.Claim.ClaimType == "UserType" && x.Claim.ClaimValue == UserTypes.Student)
                .Select(x => x.User)
                .AsNoTracking()
                .ToListAsync();

            return View("Index", students);
        }

        // Hiển thị danh sách lớp học mà học viên đó tham gia
        [HttpGet]
        [Authorize(Policy = "IsStudent")]
        public async Task<IActionResult> ClassesEnroll()
        {
            var student = await _userManager.GetUserAsync(User);
            if (student == null)
            {
                return Unauthorized();
            }
            // Define the priority order for ClassStatus
            var statusOrder = new[]
            {
                 ClassStatus.InProgress,
                 ClassStatus.PaymentPending,
                 ClassStatus.Archived
            };
            // Fetch classes attended by the student
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.ClassSchedules)
                .Include(c => c.StudentGrades)
                .Where(c => c.Students.Any(s => s.Id == student.Id))
                .ToListAsync();

            // Sort classes by status order
            classes = classes.OrderBy(c => Array.IndexOf(statusOrder, c.Status)).ToList();

            var classesEnroll = new List<ClassesEnrollViewModel>();
            int scheduleDataId = 1;

            foreach (var aClass in classes)
            {
                var viewModel = new ClassesEnrollViewModel
                {
                    ClassId = aClass.Id,
                    ClassName = aClass.Name,
                    CourseName = aClass.Course.Name,
                    RoomNumber = aClass.RoomNumber,
                    Schedule = aClass.ClassSchedules,
                    ScheduleData = new List<ScheduleData>(),
                    Status = aClass.Status,
                    StartDate = aClass.StartDate.ToString("MM/dd/yyyy"),
                    EndDate = aClass.EndDate.ToString("MM/dd/yyyy"),
                    HasGrades = aClass.StudentGrades.Any(sg => sg.StudentId == student.Id)
                };

                // Generate ScheduleData based on ClassSchedule
                var startDate = aClass.StartDate.ToDateTime(TimeOnly.MinValue);
                var endDate = aClass.EndDate.ToDateTime(TimeOnly.MinValue);

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    foreach (var cs in aClass.ClassSchedules)
                    {
                        if (date.DayOfWeek == cs.Day)
                        {
                            var startDateTime = date.Date.Add(cs.StartTime.ToTimeSpan());
                            var endDateTime = date.Date.Add(cs.EndTime.ToTimeSpan());

                            viewModel.ScheduleData.Add(new ScheduleData
                            {
                                Id = scheduleDataId++,
                                Subject = aClass.Name,
                                StartTime = startDateTime,
                                EndTime = endDateTime
                            });
                        }
                    }
                }

                classesEnroll.Add(viewModel);
            }

            return View("ClassesEnroll", classesEnroll);
        }

        [HttpGet]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public IActionResult Add()
        {
            return View(new StudentViewModel());
        }

        [HttpPost]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public async Task<IActionResult> Add(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = UserTypes.Student,
                    // Tạm thời set EmailConfirmed = true để không cần xác nhận email
                    EmailConfirmed = true

                };
                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password is required.");
                    return View(model);
                }
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddClaimAsync(user, new Claim("UserType", UserTypes.Student));
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public async Task<IActionResult> Edit(string id)
        {
            var student = await _userManager.FindByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name ?? string.Empty,
                Email = student.Email ?? string.Empty,
                PhoneNumber = student.PhoneNumber ?? string.Empty
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "CanManageStudentsLectures")]
        public async Task<IActionResult> Edit(StudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Id == null)
            {
                return NotFound();
            }

            // Update existing student
            var existingStudent = await _userManager.FindByIdAsync(model.Id);
            if (existingStudent == null)
            {
                return NotFound();
            }
            var student = existingStudent;

            student.Name = model.Name;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(student);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Policy = "CanDeleteStudentsLectures")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var student = await _userManager.FindByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            _context.Users.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Xem bảng điểm một lớp của học viên
        [HttpGet]
        [Authorize(Policy = "IsStudent")]
        public async Task<IActionResult> Grades(int classId)
        {
            var student = await _userManager.GetUserAsync(User);

            if (student == null)
            {
                return Unauthorized();
            }

            var studentGrades = await _context.StudentGrades
                .Include(sg => sg.Class)
                    .ThenInclude(c => c.Course)
                .Include(sg => sg.Class)
                    .ThenInclude(c => c.Lecturers)
                .Where(sg => sg.ClassId == classId && sg.StudentId == student.Id)
                .ToListAsync();

            return View(studentGrades);
        }
        
        // Xem bảng điểm các lớp của một học viên
        [HttpGet]
        [Authorize(Policy = "IsStudent")]
        public async Task<IActionResult> AllGrades()
        {
            var student = await _userManager.GetUserAsync(User);

            if (student == null)
            {
                return Unauthorized();
            }

            var studentGrades = await _context.StudentGrades
                .Include(sg => sg.Class)
                .ThenInclude(c => c.Course)
                .Include(sg => sg.Class)
                .ThenInclude(c => c.Lecturers)
                .Where(sg => sg.StudentId == student.Id)
                .ToListAsync();

            return View(studentGrades);
        }
        
        //Xem lịch thi của học viên
        [HttpGet]
        [Authorize(Policy = "IsStudent")]
        public async Task<IActionResult> CurrentExamSchedule()
        {
            var student = await _userManager.GetUserAsync(User);
            
            if (student == null)
            {
                return Unauthorized();
            }

            // Fetch InProgress classes of the student
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.ExamSchedules)
                .Where(c => c.Status == ClassStatus.InProgress && c.Students.Any(s => s.Id == student.Id))
                .ToListAsync();
            return View("CurrentExamSchedule", classes);
        }
        
        // Xem hoá đơn của học viên
        [HttpGet]
        [Authorize(Policy = "IsStudent")]
        public async Task<IActionResult> GetInvoices()
        {
            var student = await _userManager.GetUserAsync(User);
            if (student == null)
            {
                return Unauthorized();
            }

            var invoices = await _context.Invoices
                .Include(i => i.Class)
                .ThenInclude(c => c.Course)
                .Where(i => i.StudentId == student.Id)
                .ToListAsync();

            return View("ListInvoices", invoices);
        }
        
        [HttpGet]
        [Authorize(Policy = "IsStudent")]
        public async Task<IActionResult> PayInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null || invoice.StudentId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            // Implement payment logic here

            invoice.Status = InvoiceStatus.Paid;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thanh toán thành công";
            return RedirectToAction("GetInvoices");
        }

        [HttpGet]
        [Authorize(Policy = "IsStudent")]
        public async Task<IActionResult> InvoiceDetails(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Class)
                .ThenInclude(c => c.Course)
                .Include(i => i.Student)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null || invoice.StudentId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View("InvoiceDetails", invoice);
        }
        
        [HttpGet]
    [Authorize(Policy = "IsStudent")]
    public async Task<IActionResult> ExportToPdf(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Class)
                .ThenInclude(c => c.Course)
            .Include(i => i.Student)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null || invoice.StudentId != _userManager.GetUserId(User))
        {
            return NotFound();
        }

        var htmlContent = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    .invoice-box {{ max-width: 800px; margin: auto; padding: 30px; border: 1px solid #eee; box-shadow: 0 0 10px rgba(0, 0, 0, 0.15); }}
                    .invoice-box table {{ width: 100%; line-height: inherit; text-align: left; }}
                    .invoice-box table td {{ padding: 5px; vertical-align: top; }}
                    .invoice-box table tr td:nth-child(2) {{ text-align: right; }}
                    .invoice-box table tr.top table td {{ padding-bottom: 20px; }}
                    .invoice-box table tr.information table td {{ padding-bottom: 40px; }}
                    .invoice-box table tr.heading td {{ background: #eee; border-bottom: 1px solid #ddd; font-weight: bold; }}
                    .invoice-box table tr.details td {{ padding-bottom: 20px; }}
                    .invoice-box table tr.item td {{ border-bottom: 1px solid #eee; }}
                    .invoice-box table tr.item.last td {{ border-bottom: none; }}
                    .invoice-box table tr.total td:nth-child(2) {{ border-top: 2px solid #eee; font-weight: bold; }}
                </style>
            </head>
            <body>
                <div class='invoice-box'>
                    <table cellpadding='0' cellspacing='0'>
                        <tr class='top'>
                            <td colspan='2'>
                                <table>
                                    <tr>
                                        <td class='title'>
                                            <h2>Hóa đơn</h2>
                                        </td>
                                        <td>
                                            Hóa đơn #: {invoice.Id}<br>
                                            Ngày tạo: {invoice.CreatedDate:dd/MM/yyyy}<br>
                                            Ngày cập nhật: {invoice.UpdatedDate:dd/MM/yyyy}
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr class='information'>
                            <td colspan='2'>
                                <table>
                                    <tr>
                                        <td>
                                            Tên lớp học: {invoice.Class.Name}<br>
                                            Tên môn học: {invoice.Class.Course.Name}
                                        </td>
                                        <td>
                                            Học viên: {invoice.Student.Name}<br>
                                            Email: {invoice.Student.Email}
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr class='heading'>
                            <td>Thông tin</td>
                            <td>Chi tiết</td>
                        </tr>
                        <tr class='item'>
                            <td>Số tiền</td>
                            <td>{invoice.Amount.ToString("C0", new System.Globalization.CultureInfo("vi-VN"))}</td>
                        </tr>
                        <tr class='item'>
                            <td>Trạng thái</td>
                            <td>{(invoice.Status == InvoiceStatus.Unpaid ? "Chưa thanh toán" : "Đã thanh toán")}</td>
                        </tr>
                    </table>
                </div>
            </body>
            </html>";

        var pdfBytes = _pdfConverter.ConvertToPdf(htmlContent);

        return File(pdfBytes, "application/pdf", $"Invoice_{invoice.Id}.pdf");
    }
        
    }
}