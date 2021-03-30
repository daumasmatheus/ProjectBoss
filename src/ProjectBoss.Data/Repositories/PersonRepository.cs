using Microsoft.EntityFrameworkCore;
using ProjectBoss.Core.Entities;
using ProjectBoss.Data.DatabaseContext;
using ProjectBoss.Data.Repositories.Base;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories
{
    public class PersonRepository : RepositoryBase<Person>, IPersonRepository
    {
        public PersonRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public async Task<List<Person>> GetAllPersonWithUser()
            => await dbContext.Person.Include(p => p.User)
                                     .Where(x => !x.User.IsAdmin)
                                     .ToListAsync();

        public async Task<Person> GetPersonWithChildEntities(Guid personId)
            => await dbContext.Person.Where(x => x.PersonId == personId)
                                     .Include(rel => rel.Tasks)
                                     .Include(rel => rel.Projects)
                                     .Include(rel => rel.Comments)
                                     .Include(rel => rel.PersonInProject)
                                     .FirstOrDefaultAsync();

        public async Task<Person> GetPersonWithChildEntities(string userId)
        => await dbContext.Person.Where(x => x.UserId == userId)
                                     .Include(rel => rel.Tasks)
                                     .Include(rel => rel.Projects)
                                     .Include(rel => rel.Comments)
                                     .Include(rel => rel.PersonInProject)
                                     .FirstOrDefaultAsync();
    }
}
