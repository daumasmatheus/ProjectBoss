using ProjectBoss.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories.Interfaces
{
    public interface IPersonInProjectRepository : IRepositoryBase<PersonInProject>
    {
        Task<IEnumerable<PersonInProject>> GetProjectsByPersonWithProjectData(Guid personId);
        Task<IEnumerable<PersonInProject>> GetProjectAttendants(Guid projectId);
        Task<IEnumerable<PersonInProject>> GetOpenPersonProjectsWithChildEntities(Guid projectId);
        Task<IEnumerable<PersonInProject>> GetAllWithChildEntities();
    }
}
