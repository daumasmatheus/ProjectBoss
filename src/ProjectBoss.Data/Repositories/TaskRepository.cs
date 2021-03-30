using Microsoft.EntityFrameworkCore;
using ProjectBoss.Core.Entities;
using ProjectBoss.Data.DatabaseContext;
using ProjectBoss.Data.Repositories.Base;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Task = ProjectBoss.Core.Entities.Task;

namespace ProjectBoss.Data.Repositories
{
    public class TaskRepository : RepositoryBase<Task>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public async Task<List<Core.Entities.Task>> GetAllTasksWithChildEntities()
         => await dbContext.Task.Include(rel => rel.Attendant)
                                .Include(rel => rel.Author)
                                .Include(rel => rel.Status)
                                .Include(rel => rel.Priority)
                                .ToListAsync();

        public async Task<List<Core.Entities.Task>> GetAllActiveTasksWithChildEntities()
         => await dbContext.Task.Include(rel => rel.Attendant)
                                .Include(rel => rel.Author)
                                .Include(rel => rel.Status)
                                .Include(rel => rel.Priority)
                                .Include(rel => rel.Project)
                                .Where(x => !x.Removed)
                                .ToListAsync();

        public async Task<List<Core.Entities.Task>> GetAllTasksByUserIdWithChildEntities(string userId)
            => await dbContext.Task.Include(rel => rel.Attendant)
                                   .Include(rel => rel.Author)
                                   .Include(rel => rel.Status)
                                   .Include(rel => rel.Priority)
                                   .Where(x => x.Author.UserId == userId.ToUpper())
                                   .ToListAsync();

        public async Task<Core.Entities.Task> GetTaskByTaskIdWithChildEntities(Guid taskId)
            => await dbContext.Task.Include(rel => rel.Attendant)
                                   .Include(rel => rel.Author)
                                   .Include(rel => rel.Status)
                                   .Include(rel => rel.Priority)
                                   .Where(x => x.TaskId == taskId)
                                   .FirstOrDefaultAsync();

        public async Task<List<Core.Entities.Task>> GetTasksByAttendant(Guid attendantId)
             => await dbContext.Task.Include(rel => rel.Attendant)
                                    .Include(rel => rel.Author)
                                    .Include(rel => rel.Status)
                                    .Include(rel => rel.Priority)
                                    .Include(rel => rel.Project)
                                    .Where(x => x.Attendant.PersonId == attendantId)
                                    .ToListAsync();

        public async Task<List<Task>> GetTasksByAuthor(Guid authorId)
            => await dbContext.Task.Include(rel => rel.Attendant)
                                   .Include(rel => rel.Author)
                                   .Include(rel => rel.Status)
                                   .Include(rel => rel.Priority)
                                   .Include(rel => rel.Project)
                                   .Where(x => x.Author.PersonId == authorId && !x.Removed)
                                   .ToListAsync();

        public async Task<List<Task>> GetTasksByProjectIdWithChildEntities(Guid projectId)
        {
            return await dbContext.Task.Where(x => x.ProjectId == projectId)
                                       .Include(rel => rel.Author)
                                       .Include(rel => rel.Attendant)
                                       .Include(rel => rel.Comments)
                                       .Include(rel => rel.Status)
                                       .Include(rel => rel.Priority)
                                       .Include(rel => rel.Project)
                                       .ToListAsync();
        }        
    }
}
