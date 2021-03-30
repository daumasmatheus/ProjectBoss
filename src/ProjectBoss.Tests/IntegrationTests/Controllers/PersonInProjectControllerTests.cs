using NUnit.Framework;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Core.Entities;
using ProjectBoss.Tests.IntegrationTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ProjectBoss.Tests.IntegrationTests.Controllers
{
    [TestFixture]
    public class PersonInProjectControllerTests : HttpRequestIntegrationTestsBase
    {
        ProjectDto testProject;
        PersonBasicDto taskAttendant;
        CreateProjectTaskDto projectTask;

        public PersonInProjectControllerTests()
        {
            taskAttendant = mapper.Map<PersonBasicDto>(dbContext.Person.ToList().FirstOrDefault());
            projectTask = new CreateProjectTaskDto
            {
                Attendant = taskAttendant,
                AuthorId = ADMIN_PERSON_ID,
                StatusId = 1,
                PriorityId = 1,
                Title = "Task 1",
                Description = "Task 1"
            };

            testProject = new ProjectDto
            {
                AuthorId = ADMIN_PERSON_ID,
                Title = "New Project",
                Description = "New Project",
                StartDate = DateTime.Now.Date,
                ConclusionDate = DateTime.Now.AddMonths(1),
                AttendantIds = new List<Guid> { ADMIN_PERSON_ID },
                Tasks = new List<CreateProjectTaskDto>
                {
                    projectTask
                }
            };
        }

        [OneTimeSetUp]
        public async System.Threading.Tasks.Task Setup()
        {
            dbContext.Project.Add(mapper.Map<Project>(testProject));

            foreach (var attendantId in testProject.AttendantIds)
                dbContext.PersonInProject.Add(new Domain.Entities.PersonInProject { PersonId = attendantId, ProjectId = testProject.ProjectId });

            await dbContext.SaveChangesAsync();
        }

        [Test]
        public async System.Threading.Tasks.Task GetAssignedProjects_GivenAValidPersonIdThatHasProjectAssigneds_ShouldReturnAsignedProjects()
        {
            var request = await DoGetRequest("api/personinproject/getassignedprojects", $"personId={ADMIN_PERSON_ID}");
            request.EnsureSuccessStatusCode();

            var response = GetMultipleResults<PersonInProjectSimpleDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(response.Count > 0);
        }

        [Test]
        public async System.Threading.Tasks.Task GetAssignedProjects_GivenAValidPersonIdThatHasNoProjectAssigneds_ShouldReturnNoContent()
        {
            var request = await DoGetRequest("api/personinproject/getassignedprojects", $"personId={Guid.NewGuid()}");

            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetAssignedProjects_GivenAnInvalidUserId_ShouldBadRequest()
        {
            var request = await DoGetRequest("api/personinproject/getassignedprojects", $"personId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectAttendants_GivenAValidProjectId_ShouldReturnItsAttendants()
        {
            var request = await DoGetRequest("api/personinproject/getprojectattendants", $"projectId={testProject.ProjectId}");
            request.EnsureSuccessStatusCode();

            var result = GetMultipleResults<PersonFullDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectAttendants_GivenAValidProjectIdWithNoAttendants_ShouldReturnNoContent()
        {
            var request = await DoGetRequest("api/personinproject/getprojectattendants", $"projectId={Guid.NewGuid()}");

            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectAttendants_GivenAnInvalidProjectId_ShouldReturnBadRequest()
        {
            var request = await DoGetRequest("api/personinproject/getprojectattendants", $"projectId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task RemoveProjectAttendants_GivenValidProjectIdAndAttendantId_ShouldRemoveAttendant()
        {
            var request = await DoPostRequest("api/personinproject/removeprojectattendants",
                                              new PersonInProjectParameterDto { ProjectId = testProject.ProjectId, AttendantIds = new Guid[] { taskAttendant.PersonId } });
            request.EnsureSuccessStatusCode();

            var response = GetStructResult<bool>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(response);
        }

        [Test]
        public async System.Threading.Tasks.Task RemoveProjectAttendants_GivenInvalidParameters_ShouldReturnBadRequest()
        {
            var request = await DoPostRequest("api/personinproject/removeprojectattendants",
                                              new PersonInProjectParameterDto { ProjectId = Guid.Empty, AttendantIds = new Guid[] { Guid.Empty } });

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task AddProjectAttendant_GivenValidProjectIdAndAttendantId_ShouldAddNewAttendant()
        {
            var request = await DoPostRequest("api/personinproject/addprojectattendant",
                                              new PersonInProjectParameterDto { ProjectId = testProject.ProjectId, AttendantIds = new Guid[] { USER2_PERSON_ID } });
            request.EnsureSuccessStatusCode();

            var response = GetStructResult<bool>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(response);
        }

        [Test]
        public async System.Threading.Tasks.Task AddProjectAttendant_GivenInvalidParameters_ShouldReturnBadRequest()
        {
            var request = await DoPostRequest("api/personinproject/addprojectattendant",
                                              new PersonInProjectParameterDto { ProjectId = Guid.Empty, AttendantIds = new Guid[] { Guid.Empty } });

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task AddProjectAttendant_GivenUnexistentProjectId_ShouldReturnBadRequest()
        {
            var request = await DoPostRequest("api/personinproject/addprojectattendant",
                                              new PersonInProjectParameterDto { ProjectId = Guid.NewGuid(), AttendantIds = new Guid[] { USER2_PERSON_ID } });


            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task AddProjectAttendant_GivenUnexistentAttendantId_ShouldReturnBadRequest()
        {
            var request = await DoPostRequest("api/personinproject/addprojectattendant",
                                              new PersonInProjectParameterDto { ProjectId = testProject.ProjectId, AttendantIds = new Guid[] { Guid.NewGuid() } });


            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }
    }
}
