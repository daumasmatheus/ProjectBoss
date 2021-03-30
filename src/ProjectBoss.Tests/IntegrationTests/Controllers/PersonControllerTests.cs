using NUnit.Framework;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Tests.IntegrationTests.Base;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.IntegrationTests.Controllers
{
    [TestFixture]
    public class PersonControllerTests : HttpRequestIntegrationTestsBase
    {
        [Test]
        public async Task GetAllPerson_ThereIsPersonDataOnDb_ShouldGetAllRecords()
        {
            var request = await DoGetRequest("api/person/getall", "");
            request.EnsureSuccessStatusCode();

            var result = GetMultipleResults<PersonFullDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public async Task GetPersonData_ThereIsAPersonWithGivenPersonId_ShouldGetThePersonRecord()
        {
            var request = await DoGetRequest("api/person/getpersondata", $"personId={ADMIN_PERSON_ID}");
            request.EnsureSuccessStatusCode();

            var result = GetSingleResult<PersonFullDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.NotNull(result);
            Assert.AreEqual(ADMIN_PERSON_ID, result.PersonId);
        }

        [Test]
        public async Task GetPersonData_ThereIsntAPersonWithGivenPersonId_ReturnNoContent()
        {
            var request = await DoGetRequest("api/person/getpersondata", $"personId={Guid.NewGuid()}");

            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async Task GetPersonData_ThePersonIdIsGuidEmpty_ReturnBadRequest()
        {
            var request = await DoGetRequest("api/person/getpersondata", $"personId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }
    }
}
