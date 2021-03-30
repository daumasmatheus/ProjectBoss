using ProjectBoss.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services.Interfaces
{
    public interface ICommentService
    {
        Task<NewCommentDto> NewComment(NewCommentDto comment);
        Task<IEnumerable<CommentDto>> GetCommentsByTask(Guid taskId);
    }
}
