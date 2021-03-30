using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectBoss.Api.Controllers.Base;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Extensions;
using ProjectBoss.Api.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Controllers
{
    [Route("api/Person")]
    [ApiController]
    [AllowAnonymous]
    public class PersonController : BaseController
    {
        private readonly IPersonService personService;

        public PersonController(IPersonService _personService)
        {
            personService = _personService;
        }

        /// <summary>
        /// Edita os dados de uma pessoa
        /// </summary>
        /// <param name="editPerson">Objeto contendo os dados editados</param>
        /// <returns></returns>
        [HttpPatch("EditPerson")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> EditPerson(EditPersonDataDto editPerson)
        {
            try
            {
                var result = await personService.EditPerson(editPerson);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Obtem todos os registros de pessoas
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetAllPerson()
        {
            try
            {
                var result = await personService.GetAllPerson();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Obtem os dados de uma pessoa
        /// </summary>
        /// <param name="personId">Id da pessoa</param>
        /// <returns></returns>
        [HttpGet("GetPersonData")]
        [MutiplePoliciesAuthorize("RequireUser;RequireProjectManager;RequireAdministrator")]
        public async Task<IActionResult> GetPersonData(Guid personId)
        {
            try
            {
                if (personId == null || personId == Guid.Empty)
                    return BadRequest();

                var result = await personService.GetPersonalDataByPersonId(personId);

                if (result == null)
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }        
    }
}
