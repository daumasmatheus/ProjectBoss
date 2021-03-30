using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories.Interfaces
{
    public interface ITaskRepository : IRepositoryBase<Core.Entities.Task>
    {
        Task<List<Core.Entities.Task>> GetAllTasksWithChildEntities();
        Task<List<Core.Entities.Task>> GetTasksByProjectIdWithChildEntities(Guid projectId);
        Task<List<Core.Entities.Task>> GetAllActiveTasksWithChildEntities();
        Task<List<Core.Entities.Task>> GetAllTasksByUserIdWithChildEntities(string userId);
        Task<Core.Entities.Task> GetTaskByTaskIdWithChildEntities(Guid taskId);
        Task<List<Core.Entities.Task>> GetTasksByAttendant(Guid attendantId);
        Task<List<Core.Entities.Task>> GetTasksByAuthor(Guid authorId);
    }
}
