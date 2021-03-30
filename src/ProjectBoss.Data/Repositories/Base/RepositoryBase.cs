using Microsoft.EntityFrameworkCore;
using ProjectBoss.Data.DatabaseContext;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories.Base
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected ApplicationDbContext dbContext;

        public RepositoryBase(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task Create(T entity)
        {
            await dbContext.Set<T>().AddAsync(entity);
        }

        public async Task Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetManyByCondition(Expression<Func<T, bool>> expression)
        {
            return await dbContext.Set<T>().AsNoTracking()
                                           .Where(expression)
                                           .ToListAsync();
        }

        public async Task<T> GetSingleByCondition(Expression<Func<T, bool>> expression)
        {
            return await dbContext.Set<T>().AsNoTracking()
                                           .Where(expression)
                                           .FirstOrDefaultAsync();
        }

        public async Task Update(T original, T updated)
        {
            dbContext.Entry(original).CurrentValues.SetValues(updated);
        }

        public async Task Update(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }

        public async Task<bool> SaveChanges()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }        
    }
}
