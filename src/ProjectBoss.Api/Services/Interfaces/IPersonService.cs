using ProjectBoss.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services.Interfaces
{
    public interface IPersonService
    {
        Task<List<PersonFullDto>> GetAllPerson();
        Task<bool> CreatePerson(CreatePersonDto person);
        Task<PersonFullDto> GetPersonalDataByUserId(string userId);
        Task<PersonFullDto> GetPersonalDataByPersonId(Guid personId);
        Task<bool> EditPerson(EditPersonDataDto editPerson);
    }
}
