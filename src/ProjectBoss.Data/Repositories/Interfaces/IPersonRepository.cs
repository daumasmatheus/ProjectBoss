using ProjectBoss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories.Interfaces
{
    public interface IPersonRepository : IRepositoryBase<Person>
    {
        Task<List<Person>> GetAllPersonWithUser();
        Task<Person> GetPersonWithChildEntities(Guid personId);
        Task<Person> GetPersonWithChildEntities(string userId);
    }
}
