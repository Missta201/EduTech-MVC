using EduTech.Models;
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
    }
}
