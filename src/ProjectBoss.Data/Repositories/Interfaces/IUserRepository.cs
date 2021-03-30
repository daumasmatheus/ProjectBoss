using Microsoft.AspNetCore.Identity;
using ProjectBoss.Domain.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories.Interfaces
{
    public interface IUserRepository : IRepositoryBase<ApplicationUser>
    {
        Task<IEnumerable<ApplicationUser>> GetUsers();
        Task<ApplicationUser> GetUserById(string userId);
        Task<IdentityRole> GetUserRole(string userId);        
    }
}
