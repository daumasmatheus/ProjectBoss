using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Tests.IntegrationTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.IntegrationTests.Controllers
{
    [TestFixture]
    public class UserControllerTests : HttpRequestIntegrationTestsBase
    {
        [Test]
        public async Task GetUsers_ThereIsUsersToReturn_ShouldReturnTheUsers()
        {
            var request = await DoGetRequest("api/user/getusers", "");
            request.EnsureSuccessStatusCode();

            var result = GetMultipleResults<UserViewDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(result.Count > 0);
        }     
        
        [Test]
        public async Task GetUserById_GivenAValidAndExistingUserId_ShouldReturnTheUser()
        {
            var request = await DoGetRequest("api/user/getuserbyid", $"userId={ADMIN_USER_ID}");
            request.EnsureSuccessStatusCode();

            var result = GetSingleResult<UserViewDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(result.PersonId == ADMIN_PERSON_ID);
        }

        [Test]
        public async Task GetUserById_GivenAValidAndUnexistingUserId_ShouldReturnNoContent()
        {
            var request = await DoGetRequest("api/user/getuserbyid", $"userId={Guid.NewGuid()}");            

            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async Task GetUserById_GivenAnInvalidUserId_ShouldReturnBadRequest()
        {
            var request = await DoGetRequest("api/user/getuserbyid", $"userId={string.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async Task EditUser_GivenAValidUserDataToEdit_ShouldEditTheUserData()
        {
            var userdata = new UserViewDto
            {
                Id = ADMIN_USER_ID.ToString(),                
                Email = "edited@pjb.com",
                UserName = "edited@pjb.com",
            };

            var request = await DoPostRequest("api/user/edituser", userdata);
            request.EnsureSuccessStatusCode();

            var response = GetStructResult<bool>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(response);
        }

        [Test]
        public async Task EditUser_GivenAInvalidUserDataToEdit_ShouldReturnBadRequest()
        {
            var request = await DoPostRequest("api/user/edituser", null);

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async Task ResetUserPassword_GivenAValidAndExistingUserId_ShouldResetUserPassword()
        {
            var request = await DoPostRequest("api/user/resetuserpassword", ADMIN_USER_ID.ToString());
            request.EnsureSuccessStatusCode();

            var response = GetStructResult<bool>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(response);
        }

        [Test]
        public async Task ResetUserPassword_GivenAnInvalidUserId_ShouldReturnBadRequest()
        {
            var request = await DoPostRequest("api/user/resetuserpassword", null);

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async Task ExportUsersAsXlsx_ThereIsUsersOnApplication_ShouldExportTheData()
        {
            var request = await DoGetRequest("api/user/ExportUsersAsXlsx", "");
            request.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.NotNull(await request.Content.ReadAsStringAsync());
            Assert.IsTrue(request.Content.Headers.ContentType.MediaType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Test]
        public async Task ExportUsersAsPdf_ThereIsUsersOnApplication_ShouldExportTheData()
        {
            var request = await DoGetRequest("api/user/ExportUsersAsPdf", "");
            request.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.NotNull(await request.Content.ReadAsStringAsync());
            Assert.IsTrue(request.Content.Headers.ContentType.MediaType == "application/pdf");
        }
    }
}
