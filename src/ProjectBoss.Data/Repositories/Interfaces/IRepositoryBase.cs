using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetManyByCondition(Expression<Func<T, bool>> expression);
        Task<T> GetSingleByCondition(Expression<Func<T, bool>> expression);
        Task Create(T entity);
        Task Update(T original, T updated);
        Task Update(T entity);
        Task Delete(T entity);
        Task<bool> SaveChanges();
    }
}
