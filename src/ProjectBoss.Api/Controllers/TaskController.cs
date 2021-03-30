using Microsoft.AspNetCore.Mvc;
using ProjectBoss.Api.Controllers.Base;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Extensions;
using ProjectBoss.Api.Services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Controllers
{
    [Route("api/Task")]
    [ApiController]
    public class TaskController : BaseController
    {
        private readonly ITaskService taskService;

        public TaskController(ITaskService _taskService)
        {
            taskService = _taskService;
        }

        /// <summary>
        /// Recupera as tarefas pelo Id do autor
        /// </summary>
        /// <param name="authorId">Id do autor da tarefa</param>
        /// <returns></returns>
        [HttpGet("GetTasksByAuthor")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetTasksByAuthor(Guid authorId)
        {
            try
            {
                if (authorId == null || authorId == Guid.Empty)
                    return BadRequest();

                var result = await taskService.GetTasksByAuthorId(authorId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return HandleException(e.Message);
            }
        }

        /// <summary>
        /// Recupera as tarefas pelo responsavel pela mesma
        /// </summary>
        /// <param name="attendantId">Id do responsável pela tarefa</param>
        /// <returns></returns>
        [HttpGet("GetTasksByAttendant")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetTasksByAttendant(Guid attendantId)
        {
            try
            {
                if (attendantId == null || attendantId == Guid.Empty)
                    return BadRequest();

                var result = await taskService.GetTasksByAttendant(attendantId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return HandleException(e.Message);
            }
        }

        /// <summary>
        /// Recupera todas as tarefas ativas (não-concluídas)
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllActiveTasks")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetAllActiveTasks()
        {
            try
            {
                var result = await taskService.GetAllActiveTasks();

                return Ok(result);
            }
            catch (Exception e)
            {
                return HandleException(e.Message);
            }
        }

        /// <summary>
        /// Recupera as tarefas pelo Id do projeto
        /// </summary>
        /// <param name="projectId">Id do projeto</param>
        /// <returns></returns>
        [HttpGet("GetTasksByProjectId")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetTasksByProjectId([FromQuery] Guid projectId)
        {
            try
            {
                if (projectId == null || projectId == Guid.Empty)
                    return BadRequest();

                var result = await taskService.GetTasksByProjectId(projectId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Cria uma nova tarefa
        /// </summary>
        /// <param name="task">Dados da tarefa a ser criada</param>
        /// <returns></returns>
        [HttpPost("NewTask")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> NewTask(CreateTaskDto task)
        {
            try
            {
                var result = await taskService.CreateNewTask(task);

                if (result == null)
                    return BadRequest();

                return Ok(result);
            }
            catch (Exception e)
            {
                return HandleException(e.Message);
            }
        }

        /// <summary>
        /// Edita uma tarefa existente
        /// </summary>
        /// <param name="task">Dados da tarefa editados</param>
        /// <returns></returns>
        [HttpPatch("EditTask")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> EditTask(EditTaskDto task)
        {
            try
            {
                if (!ModelState.IsValid)
                    return CustomResponse(ModelState);

                var result = await taskService.EditTask(task);

                return Ok(result);
            }
            catch (Exception e)
            {
                return HandleException(e.Message);
            }
        }

        /// <summary>
        /// Define a tarefa como completa
        /// </summary>
        /// <param name="taskid">Id da tarefa</param>
        /// <returns></returns>
        [HttpPost("SetTaskComplete")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> SetTaskComplete([FromBody] Guid taskid)
        {
            try
            {
                if (taskid == null || taskid == Guid.Empty)
                    return BadRequest();

                var result = await taskService.SetTaskCompleted(taskid);

                return Ok(result);
            }
            catch (Exception e)
            {
                return HandleException(e.Message);
            }
        }

        /// <summary>
        /// Alterna o status da tarefa 
        /// </summary>
        /// <param name="parameters">Objeto contendo o Id da tarefa e o status a ser definido</param>
        /// <returns></returns>
        [HttpPost("ToggleTaskStatus")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> ToggleTaskStatus(ToggleTaskStatusDto parameters)
        {
            try
            {
                if (!ModelState.IsValid)
                    return CustomResponse(ModelState);

                var result = await taskService.ToggleTaskStatus(parameters.TaskId, parameters.StatusId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return HandleException(e.Message);
            }
        }

        /// <summary>
        /// Define a tarefa como removida - exclusão logica
        /// </summary>
        /// <param name="taskId">Id da tarefa</param>
        /// <returns></returns>
        [HttpPost("RemoveTask")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> RemoveTask([FromBody] Guid taskId)
        {
            try
            {
                if (taskId == null || taskId == Guid.Empty)
                    return BadRequest();

                var result = await taskService.RemoveTask(taskId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return HandleException(e.Message);
            }
        }

        /// <summary>
        /// Cria a planilha contendo as tarefas cadastrados no sistema de deteminado usuário para download.
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <param name="restrictData">Para usuários com permissionamento de administrador, determina se o documento 
        /// irá conter apenas as tarefas criadas pelo usuário ou todas as tarefas</param>
        /// <returns></returns>
        [HttpGet("ExportTasksAsXlsx")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> ExportTasksAsXlsx(Guid userId, bool restrictData = false)
        {
            try
            {
                if (userId == null || userId == Guid.Empty)
                    return BadRequest();

                var result = await taskService.ExportTasksAsXlsl(userId, restrictData);

                return File(
                    fileContents: result,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: $"Tarefas-{DateTime.Now:dd-MM-yyyy}.xlsx"
                    );
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Cria o documento Pdf contendo as tarefas cadastradas no sistema de deteminado usuário para download.
        /// Caso o usuário tenha o permissionamento de Administrador, cria a planilha com todas as tarefas cadastradas no sistema.
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <param name="restrictData">Para usuários com permissionamento de administrador, determina se o documento 
        /// irá conter apenas as tarefas criadas pelo usuário ou todas as tarefas</param>
        /// <returns></returns>
        [HttpGet("ExportTasksAsPdf")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> ExportTasksAsPdf(Guid userId, bool restrictData = false)
        {
            try
            {
                if (userId == null || userId == Guid.Empty)
                    return BadRequest();

                var file = await taskService.ExportTasksAsPdf(userId, restrictData);
                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }
    }
}