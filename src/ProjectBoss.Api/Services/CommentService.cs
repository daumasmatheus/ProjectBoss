using AutoMapper;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services.Interfaces;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly IMapper mapper;

        public CommentService(ICommentRepository _commentRepository, IMapper _mapper)
        {
            commentRepository = _commentRepository;
            mapper = _mapper;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByTask(Guid taskId)
        {
            var comments = await commentRepository.GetTaskComments(taskId);

            if (comments == null || !comments.Any())
                return new List<CommentDto>();

            return comments.Select(x => mapper.Map<CommentDto>(x))
                           .OrderByDescending(x => x.CreatedDate);
        }

        public async Task<NewCommentDto> NewComment(NewCommentDto comment)
        {
            var entity = mapper.Map<Core.Entities.Comment>(comment);

            await commentRepository.Create(entity);

            if (await commentRepository.SaveChanges())
                return comment;
            else
                return null;
        }
    }
}
