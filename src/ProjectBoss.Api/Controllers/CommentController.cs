using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectBoss.Api.Controllers.Base;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Extensions;
using ProjectBoss.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Controllers
{
    [Route("api/Comment")]
    [ApiController]
    public class CommentController : BaseController
    {
        private readonly ICommentService commentService;

        public CommentController(ICommentService _commentService)
        {
            commentService = _commentService;
        }

        /// <summary>
        /// Obtem os comentários de uma tarefa
        /// </summary>
        /// <param name="taskId">Id da tarefa</param>
        /// <returns></returns>
        [HttpGet("GetTaskComments")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetTaskComments(Guid taskId)
        {
            try
            {
                if (taskId == null || taskId == Guid.Empty)
                    return BadRequest();

                var result = await commentService.GetCommentsByTask(taskId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Adiciona um novo comentário a uma tarefa
        /// </summary>
        /// <param name="comment">Objeto contendo o comentário a ser salvo</param>
        /// <returns></returns>
        [HttpPost("NewComment")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> NewComment(NewCommentDto comment)
        {
            try
            {
                if (!ModelState.IsValid)
                    return CustomResponse(ModelState);

                if (comment == null)
                    return BadRequest();

                var result = await commentService.NewComment(comment);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }        
    }
}
