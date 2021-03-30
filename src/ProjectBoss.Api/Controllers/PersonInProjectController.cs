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
    [Route("api/[controller]")]
    [ApiController]
    public class PersonInProjectController : BaseController
    {
        private readonly IPersonInProjectService personInProjectService;

        public PersonInProjectController(IPersonInProjectService _personInProjectService)
        {
            personInProjectService = _personInProjectService;
        }

        /// <summary>
        /// Obtem os projetos em que a pessoa é participante
        /// </summary>
        /// <param name="personId">Id da pessoa</param>
        /// <returns></returns>
        [HttpGet("GetAssignedProjects")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetAssignedProjects(Guid personId)
        {
            try
            {
                if (personId == null || personId == Guid.Empty)
                    return BadRequest();

                var result = await personInProjectService.GetProjectsWherePersonIsAttendant(personId);
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
        /// Obtem os subordinados de um projeto
        /// </summary>
        /// <param name="projectId">Id do projeto</param>
        /// <returns></returns>
        [HttpGet("GetProjectAttendants")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetProjectAttendants(Guid projectId)
        {
            try
            {
                if (projectId == null || projectId == Guid.Empty)
                    return BadRequest();

                var result = await personInProjectService.GetProjectAttendants(projectId);
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
        /// Remove um ou mais subordinado de um projeto
        /// </summary>
        /// <param name="parameter">Objeto com os parametros: Id do projeto e Ids dos subordinados</param>
        /// <returns></returns>
        [HttpPost("RemoveProjectAttendants")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> RemoveProjectAttendants(PersonInProjectParameterDto parameter)
        {
            try
            {
                if (parameter == null || !ModelState.IsValid)
                    return BadRequest();

                var result = await personInProjectService.RemoveProjectAttendant(parameter);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Adiciona um ou mais subordinado a um projeto
        /// </summary>
        /// <param name="parameter">Objeto com os parametros: Id do projeto e Ids dos subordinados</param>
        /// <returns></returns>
        [HttpPost("AddProjectAttendant")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> AddProjectAttendant(PersonInProjectParameterDto parameter)
        {
            try
            {
                if (parameter == null || !ModelState.IsValid)
                    return BadRequest();

                var result = await personInProjectService.AddProjectAttendant(parameter);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }
    }
}
