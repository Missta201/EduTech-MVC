using EduTech.Models;
using Microsoft.EntityFrameworkCore;

namespace EduTech.Services
{
    public class UserService : IUserService
    {
        private readonly EduTechDbContext _context;

        public UserService(EduTechDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }
    }
}
