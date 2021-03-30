using Microsoft.EntityFrameworkCore;
using ProjectBoss.Core.Entities;
using ProjectBoss.Data.DatabaseContext;
using ProjectBoss.Data.Repositories.Base;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories
{
    public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public async Task<Project> GetProjectById(System.Guid projectId)
        {
            return await dbContext.Project.Where(x => x.ProjectId == projectId)
                                          .Include(rel => rel.Tasks).ThenInclude(rel => rel.Author)
                                          .Include(rel => rel.Tasks).ThenInclude(rel => rel.Attendant)
                                          .Include(rel => rel.Tasks).ThenInclude(rel => rel.Status)
                                          .Include(rel => rel.Tasks).ThenInclude(rel => rel.Priority)
                                          .Include(rel => rel.Author)
                                          .Include(rel => rel.PersonInProject).ThenInclude(rel => rel.Person)
                                          .FirstOrDefaultAsync();
        }

        public async Task<Project> GetProjectDataById(System.Guid projectId)
        {
            return await dbContext.Project.Where(x => x.ProjectId == projectId)
                                          .Include(rel => rel.PersonInProject)
                                          .Include(rel => rel.Author)
                                          .FirstOrDefaultAsync();
        }

        public async Task<bool> InsertAsNoTracking(Project project)
        {
            await dbContext.Project.AddAsync(project);
            var saved = dbContext.SaveChanges() > 0;

            if (saved)
            {
                foreach (var entity in dbContext.ChangeTracker.Entries())
                {
                    entity.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }

                return saved;
            }
            else
            {
                return saved;
            }
        }        
    }
}
