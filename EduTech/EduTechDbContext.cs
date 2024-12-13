using System.Security.Claims;
using EduTech.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduTech
{
    public class EduTechDbContext : IdentityDbContext<ApplicationUser>
    {
        public EduTechDbContext(DbContextOptions<EduTechDbContext> options) : base(options)
        {
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<Class> Classes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename Identity tables
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            // Seed default users with password: Demo123@
            var adminId = Guid.NewGuid().ToString();
            var schedulerId = Guid.NewGuid().ToString();
            var lecturerId = Guid.NewGuid().ToString();

            var adminUser = new ApplicationUser
            {
                Id = adminId,
                UserName = "admin@edutech.com",
                NormalizedUserName = "ADMIN@EDUTECH.COM",
                Email = "admin@edutech.com",
                NormalizedEmail = "ADMIN@EDUTECH.COM",
                EmailConfirmed = true,
                Name = "Admin User",
                UserType = UserTypes.Admin,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var schedulerUser = new ApplicationUser
            {
                Id = schedulerId,
                UserName = "giaovu@edutech.com",
                NormalizedUserName = "GIAOVU@EDUTECH.COM",
                Email = "giaovu@edutech.com",
                NormalizedEmail = "GIAOVU@EDUTECH.COM",
                EmailConfirmed = true,
                Name = "Giáo vụ 1",
                UserType = UserTypes.Scheduler,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var lecturerUser = new ApplicationUser
            {
                Id = lecturerId,
                UserName = "giangvien@edutech.com",
                NormalizedUserName = "GIANGVIEN@EDUTECH.COM",
                Email = "giangvien@edutech.com",
                NormalizedEmail = "GIANGVIEN@EDUTECH.COM",
                EmailConfirmed = true,
                Name = "Giảng viên 1",
                UserType = UserTypes.Lecturer,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // Hash passwords
            const string defaultPassword = "Demo123@";
            var hasher = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, defaultPassword);
            schedulerUser.PasswordHash = hasher.HashPassword(schedulerUser, defaultPassword);
            lecturerUser.PasswordHash = hasher.HashPassword(lecturerUser, defaultPassword);

            // Seed default users
            builder.Entity<ApplicationUser>().HasData(adminUser, schedulerUser, lecturerUser);

            // Seed user claims
            builder.Entity<IdentityUserClaim<string>>().HasData(
                new IdentityUserClaim<string> { Id = 1, UserId = adminId, ClaimType = "UserType", ClaimValue = UserTypes.Admin },
                new IdentityUserClaim<string> { Id = 2, UserId = schedulerId, ClaimType = "UserType", ClaimValue = UserTypes.Scheduler },
                new IdentityUserClaim<string> { Id = 3, UserId = lecturerId, ClaimType = "UserType", ClaimValue = UserTypes.Lecturer }
            );
        }

    }
}
