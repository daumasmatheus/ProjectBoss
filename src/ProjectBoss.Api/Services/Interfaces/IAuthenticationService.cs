using Microsoft.AspNetCore.Identity;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Domain.Extensions;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> CreateNewUser(UserRegisterDto user, string password);
        Task<IdentityResult> ChangePassword(ChangePasswordDto changePassword);
        Task<IdentityResult> CreateNewExternalUser(ExternalUserDto externalUser);
        Task<UserLoginTokenResponseDto> GenerateJwt(string email, PersonBasicDto person);
        Task<ApplicationUser> GetUserDataByEmail(string email);
        Task<bool> AsignRole(ApplicationUser user, string role);
        Task<bool> AsignRole(string userId, string role);
        Task<bool> EditRole(string userId, string role);
        Task<bool> ResetPassword(string userId);
        Task<bool> CheckUserRole(string userId, string role);
    }
}
