using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectBoss.Api.Configuration.Extensions;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services.Interfaces;
using ProjectBoss.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AppSettings appSettings;

        public AuthenticationService(UserManager<ApplicationUser> _userManager, 
                                     IOptions<AppSettings> _appSettings,
                                     RoleManager<IdentityRole> _roleManager)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            appSettings = _appSettings.Value;
        }
        public async Task<IdentityResult> ChangePassword(ChangePasswordDto changePassword)
        {
            IdentityResult result = null;

            var user = await userManager.FindByIdAsync(changePassword.UserId);
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            result = await userManager.ResetPasswordAsync(user, token, changePassword.NewPassword);

            return result;
        }

        public async Task<bool> ResetPassword(string userId)
        {
            IdentityResult result = null;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var changePwdToken = await userManager.GeneratePasswordResetTokenAsync(user);

            result = await userManager.ResetPasswordAsync(user, changePwdToken, "123456789");

            return result.Succeeded;
        }

        public async Task<IdentityResult> CreateNewExternalUser(ExternalUserDto externalUser)
        {
            var newUser = new ApplicationUser
            {
                UserName = externalUser.Email,
                Email = externalUser.Email,
                Provider = externalUser.Provider,
                ExternalUserId = externalUser.ExternalUserId
            };

            return await userManager.CreateAsync(newUser);
        }

        public async Task<IdentityResult> CreateNewUser(UserRegisterDto userRegister, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userRegister.Email,
                Email = userRegister.Email,
                EmailConfirmed = true
            };

            return await userManager.CreateAsync(user, password);
        }

        public async Task<UserLoginTokenResponseDto> GenerateJwt(string email, PersonBasicDto person)
        {
            var user = await userManager.FindByEmailAsync(email);
            var claims = await userManager.GetClaimsAsync(user);
            var identityClaims = await GetUserClaims(claims, user);
            var encodedToken = EncodeToken(identityClaims);

            return GetTokenResponse(encodedToken, user, claims, person);
        }

        public async Task<ApplicationUser> GetUserDataByEmail(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<bool> AsignRole(ApplicationUser user, string role)
        {
            var asignableRole = roleManager.Roles.Where(r => r.Name.ToUpper() == role.ToUpper()).FirstOrDefault();

            if (asignableRole != null)
            {
                var result = await userManager.AddToRoleAsync(user, asignableRole.Name);

                return result.Succeeded ? true : false;
            }
            return false;
        }

        public async Task<bool> AsignRole(string userId, string role)
        {
            var asignableRole = roleManager.Roles.Where(r => r.Name.ToUpper() == role.ToUpper()).FirstOrDefault();

            if (asignableRole != null)
            {
                var user = await userManager.FindByIdAsync(userId);
                var result = await userManager.AddToRoleAsync(user, asignableRole.Name);

                return result.Succeeded ? true : false;
            }
            return false;
        }

        public async Task<bool> EditRole(string userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId);
            var userRoles = await userManager.GetRolesAsync(user);

            var resultRemoveRoles = await userManager.RemoveFromRolesAsync(user, userRoles);

            if (!resultRemoveRoles.Succeeded)
                return false;

            return await AsignRole(userId, role);
        }

        public async Task<bool> CheckUserRole(string userId, string role)
        {
            var roles = await userManager.GetRolesAsync(await userManager.FindByIdAsync(userId));

            return roles.Any(r => r.ToUpper() == role.ToUpper());
        }

        private UserLoginTokenResponseDto GetTokenResponse(string encodedToken, ApplicationUser user, IList<Claim> claims, PersonBasicDto person)
        {
            return new UserLoginTokenResponseDto
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(appSettings.HourToExpire).TotalSeconds,
                UserToken = new UserTokenDto
                {
                    Id = user.Id,
                    PersonId = person.PersonId,
                    Email = user.Email,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Claims = claims.Select(c => new UserClaimDto { Type = c.Type, Value = c.Value })
                }
            };
        }

        private string EncodeToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = appSettings.Emitter,
                Audience = appSettings.ValidIn,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }

        private async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, ApplicationUser user)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            claims.Add(new Claim("Provider", user.Provider));

            if (user.Provider != "LOCAL")
                claims.Add(new Claim("ExternalUserId", user.ExternalUserId.ToString()));

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }

        private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
