using EduTech.Models;
using System.Collections;

namespace EduTech.Services
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetUsersAsync();
    }
}
