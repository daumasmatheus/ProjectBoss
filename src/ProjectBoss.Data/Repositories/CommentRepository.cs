using Microsoft.EntityFrameworkCore;
using ProjectBoss.Core.Entities;
using ProjectBoss.Data.DatabaseContext;
using ProjectBoss.Data.Repositories.Base;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Comment>> GetTaskComments(Guid taskId)
        {
            var result = await dbContext.Comment
                                        .Where(x => x.TaskId == taskId)
                                        .Include(rel => rel.Person)
                                        .Include(rel => rel.Task)
                                        .ToListAsync();
            return result;
        }
    }
}
