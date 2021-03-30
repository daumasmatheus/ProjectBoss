using ProjectBoss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Data.Repositories.Interfaces
{
    public interface ICommentRepository : IRepositoryBase<Comment>
    {
        Task<IEnumerable<Comment>> GetTaskComments(Guid taskId);
    }
}
