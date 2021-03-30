using AutoMapper;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services.Interfaces;
using ProjectBoss.Data.Repositories.Interfaces;
using ProjectBoss.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services
{
    public class PersonInProjectService : IPersonInProjectService
    {
        private readonly IMapper mapper;
        private readonly IPersonInProjectRepository personInProjectRepository;
        private readonly IPersonRepository personRepository;
        private readonly IProjectRepository projectRepository;

        public PersonInProjectService(IMapper _mapper, 
                                      IPersonInProjectRepository _personInProjectRepository,
                                      IPersonRepository _personRepository,
                                      IProjectRepository _projectRepository)
        {
            mapper = _mapper;
            personInProjectRepository = _personInProjectRepository;
            personRepository = _personRepository;
            projectRepository = _projectRepository;
        }

        public async Task<bool> AddProjectAttendant(PersonInProjectParameterDto parameters)
        {
            if (await projectRepository.GetSingleByCondition(x => x.ProjectId == parameters.ProjectId) == null)
                return false;

            bool notFound = false;
            foreach (var personId in parameters.AttendantIds)
            {
                var newAttendant = await personRepository.GetSingleByCondition(x => x.PersonId == personId);
                if (newAttendant == null)
                {
                    notFound = true;
                    break;
                }
            }

            if (notFound)
                return false;

            foreach (var attendantId in parameters.AttendantIds)
            {
                PersonInProject personInProject = new PersonInProject { PersonId = attendantId, ProjectId = parameters.ProjectId };
                await personInProjectRepository.Create(personInProject);
            }

            return await personInProjectRepository.SaveChanges();
        }        

        public async Task<IEnumerable<PersonInProjectSimpleDto>> GetProjectsWherePersonIsAttendant(Guid personId)
        {
            var result = await personInProjectRepository.GetProjectsByPersonWithProjectData(personId);

            if (result == null || !result.Any())
                return new List<PersonInProjectSimpleDto>();

            return result.Select(x => mapper.Map<PersonInProjectSimpleDto>(x)).ToList();
        }        

        public async Task<IEnumerable<PersonFullDto>> GetProjectAttendants(Guid projectId)
        {
            var result = await personInProjectRepository.GetProjectAttendants(projectId);

            if (result == null || !result.Any())
                return new List<PersonFullDto>();

            var personList = result.Select(s => s.Person);

            return personList.Select(s => mapper.Map<PersonFullDto>(s));
        }

        public async Task<bool> RemoveProjectAttendant(PersonInProjectParameterDto parameters)
        {
            foreach (var personId in parameters.AttendantIds)
            {
                var entity = await personInProjectRepository.GetSingleByCondition(x => x.PersonId == personId && 
                                                                                       x.ProjectId == parameters.ProjectId);
                if (entity != null)
                    await personInProjectRepository.Delete(entity);
            }

            return await personInProjectRepository.SaveChanges();
        }
    }
}
