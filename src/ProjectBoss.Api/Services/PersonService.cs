using AutoMapper;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services.Interfaces;
using ProjectBoss.Core.Entities;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository personRepository;
        private readonly IMapper mapper;

        public PersonService(IPersonRepository _personRepository, IMapper _mapper)
        {
            personRepository = _personRepository;
            mapper = _mapper;
        }

        public async Task<bool> CreatePerson(CreatePersonDto person)
        {
            var personData = mapper.Map<Person>(person);

            await personRepository.Create(personData);

            return await personRepository.SaveChanges();
        }

        public async Task<PersonFullDto> GetPersonalDataByUserId(string userId)
        {
            var result = await personRepository.GetSingleByCondition(p => p.User.Id == userId);

            return mapper.Map<PersonFullDto>(result);
        }        

        public async Task<bool> EditPerson(EditPersonDataDto editPerson)
        {
            var entity = await personRepository.GetSingleByCondition(x => x.PersonId == editPerson.PersonId);

            if (entity == null)
                return false;

            var updated = mapper.Map(editPerson, entity);

            await personRepository.Update(updated);

            return await personRepository.SaveChanges();
        }

        public async Task<PersonFullDto> GetPersonalDataByPersonId(Guid personId)
        {
            PersonFullDto result;
            result = mapper.Map<PersonFullDto>(await personRepository.GetSingleByCondition(x => x.PersonId == personId));

            return result;
        }

        public async Task<List<PersonFullDto>> GetAllPerson()
        {
            var result = await personRepository.GetAllPersonWithUser();
            return mapper.Map<List<PersonFullDto>>(result);
        }        
    }
}
