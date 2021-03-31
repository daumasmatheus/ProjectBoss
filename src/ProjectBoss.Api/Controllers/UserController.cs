using Microsoft.AspNetCore.Mvc;
using ProjectBoss.Api.Controllers.Base;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Extensions;
using ProjectBoss.Api.Services.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService userService;

        public UserController(IUserService _userService)
        {
            userService = _userService;
        }

        /// <summary>
        /// Obtém os usuário do sistema
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUsers")]
        [MutiplePoliciesAuthorize("RequireAdministrator", true)]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var result = await userService.GetUsers();                

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Recupera o usuário pelo identificador
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <returns></returns>
        [HttpGet("GetUserById")]
        [MutiplePoliciesAuthorize("RequireAdministrator", true)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest();

                var result = await userService.GetUserById(userId);
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
        /// Edita os dados do usuário
        /// </summary>
        /// <param name="userData">Dados editados do usuário</param>
        /// <returns></returns>
        [HttpPost("EditUser")]
        [MutiplePoliciesAuthorize("RequireAdministrator", true)]
        public async Task<IActionResult> EditUser(UserViewDto userData)
        {
            try
            {
                if (userData == null)
                    return BadRequest();

                var result = await userService.EditUser(userData);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Reseta a senha do usuário para '123456789'
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <returns></returns>
        [HttpPost("ResetUserPassword")]
        [MutiplePoliciesAuthorize("RequireAdministrator", true)]
        public async Task<IActionResult> ResetUserPassword([FromBody]string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest();

                var result = await userService.ResetUserPassword(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Cria a planilha contendo os usuários cadastrados no sistema para download
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExportUsersAsXlsx")]
        [MutiplePoliciesAuthorize("RequireAdministrator", true)]
        public async Task<IActionResult> ExportUsersAsXlsx()
        {
            try
            {
                var result = await userService.DownloadUsersXlsl();                

                return File(
                    fileContents: result.ToArray(),
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: $"Usuários do sistema-{DateTime.Now:dd-MM-yyyy}.xlsx"
                    );
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Cria o documento Pdf contendo os usuários cadastrados no sistema para download
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExportUsersAsPdf")]
        [MutiplePoliciesAuthorize("RequireAdministrator", true)]
        public async Task<IActionResult> ExportUsersAsPdf()
        {
            try
            {
                var file = await userService.DownloadUsersPdf();
                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Retorna as Roles do sistema
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRoles")]
        [MutiplePoliciesAuthorize("RequireAdministrator", true)]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var result = await userService.GetRoles();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }
    }
}
