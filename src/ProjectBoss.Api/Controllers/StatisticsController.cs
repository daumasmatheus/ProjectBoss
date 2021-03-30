using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectBoss.Api.Controllers.Base;
using ProjectBoss.Api.Extensions;
using ProjectBoss.Api.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Controllers
{
    [Route("api/Statistics")]
    [ApiController]
    public class StatisticsController : BaseController
    {
        private readonly IStatisticsService statisticsService;
        public StatisticsController(IStatisticsService _statisticsService)
        {
            statisticsService = _statisticsService;
        }

        #region Person
        /// <summary>
        /// Obtem os dados para exibição na dashboard do usuario/project manager
        /// </summary>
        /// <param name="personId">Id da person associada ao usuário</param>
        /// <returns></returns>
        [HttpGet("GetPersonOverviewStatistics")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager")]
        public async Task<IActionResult> GetPersonOverviewStatistics(Guid personId)
        {
            try
            {
                if (personId == null || personId == Guid.Empty)
                    return BadRequest();

                var result = await statisticsService.GetPersonOverviewStatistics(personId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }        
        #endregion

        #region Project Statistics
        /// <summary>
        /// Obtem as tarefas em aberto e concluídas por participante por projeto
        /// </summary>
        /// <param name="projectId">Id do projeto</param>
        /// <returns></returns>
        [HttpGet("GetOpenAndOnGoingTasksByPersonInProject")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetOpenAndOnGoingTasksByPersonInProject(Guid projectId)
        {
            try
            {
                if (projectId == null || projectId == Guid.Empty)
                    return BadRequest();

                var result = await statisticsService.GetOpenAndOnGoingTasksByPersonInProject(projectId);
                if (result.Count() == 0)
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Obtem o total de tarefas organizadas por status por projeto
        /// </summary>
        /// <param name="projectId">Id do projeto</param>
        /// <returns></returns>
        [HttpGet("GetTasksStatusByProject")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetTasksStatusByProject(Guid projectId)
        {
            try
            {
                if (projectId == null || projectId == Guid.Empty)
                    return BadRequest();

                var result = await statisticsService.GetTasksStatusByProject(projectId);
                if (result.Count() == 0)
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }        

        /// <summary>
        /// Obtem o total de novas tarefas e tarefas concluídas por projeto
        /// </summary>
        /// <param name="projectId">Id do projeto</param>
        /// <returns></returns>
        [HttpGet("GetNewAndClosedTasksByDateByProject")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetNewAndClosedTasksByDateByProject(Guid projectId)
        {
            try
            {
                if (projectId == null || projectId == Guid.Empty)
                    return BadRequest();

                var result = await statisticsService.GetNewAndClosedTasksByDateByProject(projectId);
                if (result.Count() == 0)
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }        
        #endregion

        #region Admin Statistics
        /// <summary>
        /// Obtém todos os usuários criados
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCreatedUsers")]
        [MutiplePoliciesAuthorize("RequireAdministrator")]
        public async Task<IActionResult> GetCreatedUsers()
        {
            try
            {
                var result = await statisticsService.GetCreatedUsers();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Obtém todas as tarefas criadas organizadas pela data de criação
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTotalCreatedTasksByDate")]
        [MutiplePoliciesAuthorize("RequireAdministrator")]
        public async Task<IActionResult> GetTotalCreatedTasksByDate()
        {
            try
            {
                var result = await statisticsService.GetTotalCreatedTasksByDate();
                if (result == null)
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Obtém todas tarefas concluídas organizadas pela data de conclusão
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTotalConcludedTasksByDate")]
        [MutiplePoliciesAuthorize("RequireAdministrator")]
        public async Task<IActionResult> GetTotalConcludedTasksByDate()
        {
            try
            {
                var result = await statisticsService.GetTotalConcludedTasksByDate();
                if (result == null)
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Obtém todos os projetos criados organizados pela data de criação
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTotalCreatedProjectsByDate")]
        [MutiplePoliciesAuthorize("RequireAdministrator")]
        public async Task<IActionResult> GetTotalCreatedProjectsByDate()
        {
            try
            {
                var result = await statisticsService.GetTotalCreatedProjectsByDate();
                if (result == null || (string.IsNullOrEmpty(result.Name) && (result.Series == null || !result.Series.Any())))
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Obtém todos os projetos concluídos organizados pela data de conclusão
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTotalConcludedProjectsByDate")]
        [MutiplePoliciesAuthorize("RequireAdministrator")]
        public async Task<IActionResult> GetTotalConcludedProjectsByDate()
        {
            try
            {
                var result = await statisticsService.GetTotalConcludedProjectsByDate();
                if (result == null || (string.IsNullOrEmpty(result.Name) && (result.Series == null || !result.Series.Any())))
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }
        #endregion
    }
}
