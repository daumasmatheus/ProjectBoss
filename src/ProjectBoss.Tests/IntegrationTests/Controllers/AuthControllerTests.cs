using NUnit.Framework;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Tests.IntegrationTests.Base;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.IntegrationTests.Controllers
{
    [TestFixture]
    public class AuthControllerTests : HttpRequestIntegrationTestsBase
    {
        const string AUTH_REGISTER = "api/auth/register";
        const string AUTH_EXTERNAL = "api/auth/external-auth";
        const string AUTH_LOGIN = "api/auth/login";
        const string EXTERNAL_USER_ID = "191CBC5A-920F-4EF9-AE4E-0B35FDE2F26C";

        private UserRegisterDto newUser;
        private ExternalUserDto externalUser;

        [OneTimeSetUp]
        public async Task Init()
        {
            newUser = new UserRegisterDto
            {
                FirstName = "Test",
                LastName = "Testaroo",
                Email = "test@test.com",
                Password = "test123456",
                PasswordConfirmation = "test123456"
            };

            var response = await DoPostRequest(AUTH_REGISTER, newUser);
            response.EnsureSuccessStatusCode();

            externalUser = new ExternalUserDto
            {
                FirstName = "Test",
                LastName = "Test2",
                Email = "test22@external.com",
                Provider = "GOOGLE",
                ExternalUserId = EXTERNAL_USER_ID
            };

            var externalRegisterResponse = await DoPostRequest(AUTH_EXTERNAL, externalUser);
            externalRegisterResponse.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task RegisterNewUser_GivenAnModelInvalidModelState_ShouldReturnBadRequest()
        {
            var newUser = new UserRegisterDto
            {
                Email = "blavlabla",
                FirstName = "alalalaal",
                Password = "asdklçasdq2"
            };

            var response = await DoPostRequest(AUTH_REGISTER, newUser);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task RegisterNewUser_GivenAValidModelState_ShouldRegisterNewUserAndReturnUserAuthenticationToken()
        {
            newUser = new UserRegisterDto
            {
                FirstName = "Test2",
                LastName = "Testaroo",
                Email = "test2@test.com",
                Password = "test123456",
                PasswordConfirmation = "test123456"
            };

            var response = await DoPostRequest(AUTH_REGISTER, newUser);
            response.EnsureSuccessStatusCode();

            var respStr = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UserLoginTokenResponseDto>(respStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotNull(result.UserToken);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
        }

        [Test]
        public async Task UserLogin_GiveThatUserDontExist_ShouldReturnBadRequest()
        {
            var userCredentials = new UserLoginDto
            {
                Email = "test@test.com",
                Password = "22361221"
            };

            var response = await DoPostRequest(AUTH_LOGIN, userCredentials);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task UserLogin_GivenThatUserExists_ShouldReturnUserAuthenticationToken()
        {
            var userLogin = new UserLoginDto
            {
                Email = "test@test.com",
                Password = "test123456",
            };

            var loginResponse = await DoPostRequest(AUTH_LOGIN, userLogin);
            loginResponse.EnsureSuccessStatusCode();

            var loginRespString = await loginResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UserLoginTokenResponseDto>(loginRespString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.NotNull(result);
            Assert.NotNull(result.UserToken);
            Assert.AreEqual(result.UserToken.Email, userLogin.Email);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
        }

        [Test]
        public async Task ExternalUserAuthentication_GivenThatUserDoesNotExistAndModelStateIsInvalid_ShouldReturnBadRequest()
        {
            var externalUser = new ExternalUserDto
            {
                FirstName = "Test",
                LastName = "Test2"
            };

            var response = await DoPostRequest(AUTH_EXTERNAL, externalUser);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task ExternalUserAuthentication_GivenThatUserDoesNotExistAndModelStateIsValid_ShouldRegisterUserAndReturnAuthenticationToken()
        {
            externalUser = new ExternalUserDto
            {
                FirstName = "Test",
                LastName = "Test2",
                Email = "test@external.com",
                Provider = "GOOGLE",
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var response = await DoPostRequest(AUTH_EXTERNAL, externalUser);
            response.EnsureSuccessStatusCode();

            var respString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UserLoginTokenResponseDto>(respString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotNull(result.UserToken);
            Assert.AreEqual(result.UserToken.Email, externalUser.Email);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
        }

        [Test]
        public async Task ExternalUserAuthentication_GivenThatTheUserDoesExist_ShouldAuthenticateUserAndReturnLoginToken()
        {
            var externalUserLogin = new ExternalUserDto
            {
                FirstName = "Test",
                LastName = "Test2",
                Email = "test22@external.com",
                Provider = "GOOGLE",
                ExternalUserId = EXTERNAL_USER_ID
            };

            var response = await DoPostRequest(AUTH_EXTERNAL, externalUserLogin);
            response.EnsureSuccessStatusCode();

            var respString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UserLoginTokenResponseDto>(respString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotNull(result.UserToken);
            Assert.AreEqual(result.UserToken.Email, externalUser.Email);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
        }
    }
}
