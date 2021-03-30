using Microsoft.EntityFrameworkCore;
using ProjectBoss.Data.DatabaseContext;
using ProjectBoss.Data.Repositories.Base;
using ProjectBoss.Data.Repositories.Interfaces;
using ProjectBoss.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories
{
    public class PersonInProjectRepository : RepositoryBase<PersonInProject>, IPersonInProjectRepository
    {
        public PersonInProjectRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public async Task<IEnumerable<PersonInProject>> GetProjectsByPersonWithProjectData(Guid personId)
        {
            return await dbContext.PersonInProject.Where(x => x.PersonId == personId)
                                                  .Include(rel => rel.Project)
                                                  .ToListAsync();
        }

        public async Task<IEnumerable<PersonInProject>> GetProjectAttendants(Guid projectId)
        {
            return await dbContext.PersonInProject.Where(x => x.ProjectId == projectId)
                                                  .Include(rel => rel.Person).ThenInclude(rel => rel.Tasks.Where(x => x.ProjectId == projectId))
                                                  .ToListAsync();
        }

        public async Task<IEnumerable<PersonInProject>> GetOpenPersonProjectsWithChildEntities(Guid projectId)
        {
            return await dbContext.PersonInProject.Where(x => x.PersonId == projectId && !x.Project.ConcludedDate.HasValue)
                                                  .Include(rel => rel.Project)
                                                  .Include(rel => rel.Person)
                                                  .ToListAsync();
        }

        public async Task<IEnumerable<PersonInProject>> GetAllWithChildEntities()
        {
            return await dbContext.PersonInProject.Include(rel => rel.Person)
                                                  .Include(rel => rel.Project)
                                                  .ToListAsync();
        }
    }
}
