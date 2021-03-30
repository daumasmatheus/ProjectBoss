using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectBoss.Data.DatabaseContext;
using ProjectBoss.Data.Repositories.Base;
using ProjectBoss.Data.Repositories.Interfaces;
using ProjectBoss.Domain.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories
{
    public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public async Task<ApplicationUser> GetUserById(string userId)
            => await dbContext.User.Where(x => x.Id == userId).AsNoTracking().FirstOrDefaultAsync();        

        public async Task<IEnumerable<ApplicationUser>> GetUsers()
            => await dbContext.User.Where(x => !x.IsAdmin).ToListAsync();

        public async Task<IdentityRole> GetUserRole(string userId)
            => await dbContext.Roles.Where(x => x.Id == dbContext.UserRoles.Where(w => w.UserId == userId).FirstOrDefault().RoleId).FirstOrDefaultAsync();        
    }
}
