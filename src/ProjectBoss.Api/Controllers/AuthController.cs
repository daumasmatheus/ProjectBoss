using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectBoss.Api.Controllers.Base;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services.Interfaces;
using ProjectBoss.Domain.Extensions;
using System;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Controllers
{
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAuthenticationService authenticationService;
        private readonly IPersonService personService;
        private readonly IMapper mapper;

        public AuthController(SignInManager<ApplicationUser> _signInManager,
                              UserManager<ApplicationUser> _userManager,
                              IAuthenticationService _authenticationService,
                              IPersonService _personService,
                              IMapper _mapper)
        {
            signInManager = _signInManager;
            userManager = _userManager;
            authenticationService = _authenticationService;
            personService = _personService;
            mapper = _mapper;
        }

        /// <summary>
        /// Efetua o cadastro de um novo usuário
        /// </summary>
        /// <param name="userRegister">Objeto contendo os dados do novo usuário para ser cadastrado</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegister)
        {
            try
            {
                if (!ModelState.IsValid) return CustomResponse(ModelState);

                var result = await authenticationService.CreateNewUser(userRegister, userRegister.Password);

                if (result.Succeeded)
                {
                    var registeredUser = await userManager.FindByEmailAsync(userRegister.Email);

                    await authenticationService.AsignRole(registeredUser, "CommonUser");

                    var person = mapper.Map<CreatePersonDto>(userRegister);
                    person.UserId = Guid.Parse(registeredUser.Id);

                    if (await personService.CreatePerson(person))
                    {
                        var personDataBasic = mapper.Map<PersonBasicDto>(person);

                        return CustomResponse(await authenticationService.GenerateJwt(userRegister.Email, personDataBasic));
                    }
                    else
                    {
                        var createdUser = await userManager.FindByEmailAsync(userRegister.Email);
                        await userManager.DeleteAsync(createdUser);

                        AddProccessError("Falha ao registrar novo usuário");
                    }
                }

                foreach (var error in result.Errors)
                    AddProccessError(error.Description);

                return CustomResponse();
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Efetua o login do usuário
        /// </summary>
        /// <param name="userLogin">Objeto com os dados para o login do usuário</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {
            try
            {
                if (!ModelState.IsValid) return CustomResponse(ModelState);

                var result = await signInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, true);

                if (result.Succeeded)
                {
                    var userData = await authenticationService.GetUserDataByEmail(userLogin.Email);
                    var personalData = await personService.GetPersonalDataByUserId(userData.Id);

                    var personDataBasic = mapper.Map<PersonBasicDto>(personalData);

                    return CustomResponse(await authenticationService.GenerateJwt(userLogin.Email, personDataBasic));
                }

                if (result.IsLockedOut)
                {
                    AddProccessError("Usuário temporariamente bloqueado por tentativas inválidas");
                    return CustomResponse();
                }

                AddProccessError("Usuário ou senha incorretos");
                return CustomResponse();
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Efetua o login ou salva um novo cadastro de usuário por provedor externo (Google)
        /// </summary>
        /// <param name="externalUser">Objeto com os dados do usuário externo</param>
        /// <returns></returns>
        [HttpPost("external-auth")]
        public async Task<IActionResult> ExternalAuth([FromBody] ExternalUserDto externalUser)
        {
            try
            {
                var userData = await authenticationService.GetUserDataByEmail(externalUser.Email);

                if (userData != null)
                {
                    var personalData = await personService.GetPersonalDataByUserId(userData.Id);
                    var personDataBasic = mapper.Map<PersonBasicDto>(personalData);

                    return CustomResponse(await authenticationService.GenerateJwt(userData.Email, personDataBasic));
                }
                else
                {
                    var result = await authenticationService.CreateNewExternalUser(externalUser);

                    if (result.Succeeded)
                    {
                        var user = await authenticationService.GetUserDataByEmail(externalUser.Email);

                        var person = new CreatePersonDto
                        {
                            PersonId = Guid.NewGuid(),
                            FirstName = externalUser.FirstName,
                            LastName = externalUser.LastName,
                            CreatedDate = DateTime.Now,
                            UserId = Guid.Parse(user.Id)
                        };

                        if (await personService.CreatePerson(person))
                        {
                            var personDataBasic = mapper.Map<PersonBasicDto>(person);

                            return CustomResponse(await authenticationService.GenerateJwt(externalUser.Email, personDataBasic));
                        }
                        else
                        {
                            var createdUser = await userManager.FindByEmailAsync(externalUser.Email);
                            await userManager.DeleteAsync(createdUser);

                            AddProccessError("Falha ao registrar novo usuário");
                        }
                    }

                    foreach (var error in result.Errors)
                        AddProccessError(error.Description);

                    return CustomResponse();
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        /// <summary>
        /// Efetua a mudança da senha do usuário
        /// </summary>
        /// <param name="changePassword">Objeto com os dados para a mudança da senha</param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePassword)
        {
            try
            {
                if (!ModelState.IsValid) return CustomResponse(ModelState);

                var confirmCredentials = await signInManager.PasswordSignInAsync(changePassword.Email, changePassword.CurrentPassword, false, true);

                if (confirmCredentials.Succeeded)
                {
                    var result = await authenticationService.ChangePassword(changePassword);
                    return Ok(result);
                }
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }
    }
}
