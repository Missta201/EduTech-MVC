using EduTech.Models;
using Microsoft.EntityFrameworkCore;

namespace EduTech
{
    public class EduTechDbContext : DbContext
    {

        public EduTechDbContext(DbContextOptions<EduTechDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
    }
}
