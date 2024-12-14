using EduTech.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduTech.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EduTechDbContext _dbContext;
        public DbInitializer(UserManager<ApplicationUser> userManager, EduTechDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public void Initialize()
        {
            // Apply migrations if they are not applied
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    _dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while applying migrations: " + ex);
            }


            // Seed default users if they do not exist
            const string defaultPassword = "Demo123@";
            var adminUser = _userManager.FindByNameAsync("admin@edutech.com").Result;
            var schedulerUser = _userManager.FindByNameAsync("giaovu@edutech.com").Result;
            var lecturerUser = _userManager.FindByNameAsync("giangvien@edutech.com").Result;

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@edutech.com",
                    Email = "admin@edutech.com",
                    EmailConfirmed = true,
                    Name = "Admin User",
                    UserType = UserTypes.Admin
                };
                _userManager.CreateAsync(adminUser, defaultPassword).Wait();
                _userManager.AddClaimAsync(adminUser, new Claim("UserType", UserTypes.Admin)).Wait();
            }

            if (schedulerUser == null)
            {
                schedulerUser = new ApplicationUser
                {
                    UserName = "giaovu@edutech.com",
                    Email = "giaovu@edutech.com",
                    EmailConfirmed = true,
                    Name = "Giáo vụ 1",
                    UserType = UserTypes.Scheduler
                };
                _userManager.CreateAsync(schedulerUser, defaultPassword).Wait();
                _userManager.AddClaimAsync(schedulerUser, new Claim("UserType", UserTypes.Scheduler)).Wait();
            }

            if (lecturerUser == null)
            {
                lecturerUser = new ApplicationUser
                {
                    UserName = "giangvien@edutech.com",
                    Email = "giangvien@edutech.com",
                    EmailConfirmed = true,
                    Name = "Giảng viên 1",
                    UserType = UserTypes.Lecturer
                };
                _userManager.CreateAsync(lecturerUser, defaultPassword).Wait();
                _userManager.AddClaimAsync(lecturerUser, new Claim("UserType", UserTypes.Lecturer)).Wait();
            }
        }

    }
}
