using ProjectBoss.Core.Entities;
using System;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories.Interfaces
{
    public interface IProjectRepository : IRepositoryBase<Project>
    {
        Task<bool> InsertAsNoTracking(Project project);
        Task<Project> GetProjectById(Guid projectId);
        Task<Project> GetProjectDataById(Guid projectId);
    }
}
