using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectBoss.Api.Controllers.Base;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Dtos.Enums;
using ProjectBoss.Api.Extensions;
using ProjectBoss.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Controllers
{
    [Route("api/Project")]
    [ApiController]
    public class ProjectController : BaseController
    {
        private readonly IProjectService projectService;
        public ProjectController(IProjectService _projectService)
        {
            projectService = _projectService;
        }

        /// <summary>
        /// Cria um novo projeto
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPost("NewProject")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> NewProject(ProjectDto project)
        {
            try
            {
                if (project == null || !ModelState.IsValid)
                    return BadRequest();

                var result = await projectService.CreateNewProject(project);
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
        /// Obtém um projeto pelo Id
        /// </summary>
        /// <param name="projectId">Id do projeto</param>
        /// <returns></returns>
        [HttpGet("GetProjectDataById")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetProjectDataById(Guid projectId)
        {
            try
            {
                if (projectId == null || projectId == Guid.Empty)
                    return BadRequest();

                var result = await projectService.GetProjectDataById(projectId);
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
        /// Edita os dados de um projeto
        /// </summary>
        /// <param name="project">Objeto contendo os dados que foram editados</param>
        /// <returns></returns>
        [HttpPatch("EditProjectData")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> EditProjectData(ProjectDto project)
        {
            try
            {
                if (!ModelState.IsValid)
                    return CustomResponse(ModelState);

                var result = await projectService.EditProjectData(project);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Cria o documento Pdf contendo os dados do projeto para download
        /// </summary>
        /// <param name="projectId">Id do projeto</param>
        /// <returns></returns>
        [HttpGet("ExportProjectAsPdf")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> ExportProjectAsPdf(Guid projectId)
        {
            try
            {
                if (projectId == null || projectId == Guid.Empty)
                    return BadRequest();

                var file = await projectService.ExportProjectAsPdf(projectId);
                if (file == null || file.Length == 0)
                    return NoContent();

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Cria o documento Excel contendo os dados do projeto para download
        /// </summary>
        /// <param name="projectId">Id do projeto</param>
        /// <returns></returns>
        [HttpGet("ExportProjectAsXlsx")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> ExportProjectAsXlsx(Guid projectId)
        {
            try
            {
                if (projectId == null || projectId == Guid.Empty)
                    return BadRequest();

                var result = await projectService.ExportProjectAsXlsl(projectId);
                if (result == null || result.Length == 0)
                    return NoContent();

                return File(fileContents: result,
                            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                            fileDownloadName: $"Projeto-{projectId}-{DateTime.Now:dd-MM-yyyy}.xlsx");                
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Obtém dados basicos do projeto para popular o dropdown no front-end
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllProjectsForDropdown")]
        [MutiplePoliciesAuthorize("RequireAdministrator")]
        public async Task<IActionResult> GetAllProjectsForDropdown()
        {
            try
            {
                var result = await projectService.GetProjectDataForDropdown();
                if (result == null || !result.Any())
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }
        
        /// <summary>
        /// Efetua a alteração do status do projeto
        /// </summary>
        /// <param name="projectId">Id do projeto</param>
        /// <returns></returns>
        [HttpPost("ToggleProjectStatus")]
        [MutiplePoliciesAuthorize("RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> ToggleProjectStatus([FromBody]Guid projectId)
        {
            try
            {
                bool result = await projectService.ToggleProjectStatus(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }
    }
}
