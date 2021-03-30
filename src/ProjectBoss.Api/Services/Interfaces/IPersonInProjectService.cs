using ProjectBoss.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services.Interfaces
{
    public interface IPersonInProjectService 
    { 
        Task<IEnumerable<PersonInProjectSimpleDto>> GetProjectsWherePersonIsAttendant(Guid personId);
        Task<IEnumerable<PersonFullDto>> GetProjectAttendants(Guid projectId);
        Task<bool> RemoveProjectAttendant(PersonInProjectParameterDto parameters);
        Task<bool> AddProjectAttendant(PersonInProjectParameterDto parameters);
    }
}
