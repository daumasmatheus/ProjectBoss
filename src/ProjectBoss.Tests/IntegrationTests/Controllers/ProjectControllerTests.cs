using NUnit.Framework;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Core.Entities;
using ProjectBoss.Tests.IntegrationTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.IntegrationTests.Controllers
{
    [TestFixture]
    public class ProjectControllerTests : HttpRequestIntegrationTestsBase
    {
        ProjectDto newProject;
        PersonBasicDto taskAttendant;
        CreateProjectTaskDto projectTask;

        public ProjectControllerTests()
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

            newProject = new ProjectDto
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
            dbContext.Project.Add(mapper.Map<Project>(newProject));

            foreach (var attendantId in newProject.AttendantIds)
                dbContext.PersonInProject.Add(new Domain.Entities.PersonInProject { PersonId = attendantId, ProjectId = newProject.ProjectId });

            dbContext.SaveChanges();
        }

        [Test]
        public async System.Threading.Tasks.Task NewProject_ProjectDataIsValid_ShouldCreateNewProject()
        {
            newProject = new ProjectDto
            {
                AuthorId = ADMIN_PERSON_ID,
                Title = "New Project 2",
                Description = "New Project 2",
                StartDate = DateTime.Now.Date,
                ConclusionDate = DateTime.Now.AddMonths(1),
                AttendantIds = new List<Guid> { ADMIN_PERSON_ID },
                Tasks = new List<CreateProjectTaskDto>
                {
                    new CreateProjectTaskDto
                    {
                        Attendant = taskAttendant,
                        AuthorId = ADMIN_PERSON_ID,
                        StatusId = 1,
                        PriorityId = 1,
                        Title = "Task 1",
                        Description = "Task 1"
                    }
                }
            };

            var request = await DoPostRequest("api/project/newproject", newProject);
            request.EnsureSuccessStatusCode();

            var response = GetSingleResult<ProjectDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(response.ProjectId != Guid.Empty);
            Assert.IsTrue(response.ProjectId == newProject.ProjectId);
        }

        [Test]
        public async System.Threading.Tasks.Task NewProject_ProjectDataIsInvalid_ShouldReturnBadRequest()
        {
            newProject.Title = string.Empty;
            newProject.Description = string.Empty;

            var request = await DoPostRequest("api/project/newproject", newProject);

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectDataById_TheGivenProjectIdIsValidAndTheProjectExists_ShouldReturnProject()
        {
            var request = await DoGetRequest("api/project/getprojectdatabyid", $"projectId={newProject.ProjectId}");
            request.EnsureSuccessStatusCode();

            var response = GetSingleResult<ProjectDataDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(response.ProjectId == newProject.ProjectId);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectDataById_TheGivenProjectIdIsValidButThereIsNoProject_ShouldReturnNoContentResult()
        {
            var request = await DoGetRequest("api/project/getprojectdatabyid", $"projectId={Guid.NewGuid()}");
            request.EnsureSuccessStatusCode();


            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectDataById_TheGivenProjectIdIsInvalid_ShouldReturnBadRequest()
        {
            var request = await DoGetRequest("api/project/getprojectdatabyid", $"projectId={Guid.Empty}");
            
            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task EditProjectData_EditedProjectDataIsValid_ShouldEditProjectData()
        {
            newProject.Title = "edited project";
            newProject.Description = "edited description";

            var request = await DoPostRequest("api/project/editprojectdata", newProject, HttpMethod.Patch);
            request.EnsureSuccessStatusCode();

            var result = GetStructResult<bool>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task EditProjectData_EditedProjectDataIsInvalid_ShouldReturnBadRequest()
        {
            newProject.Title = string.Empty;
            newProject.Description = string.Empty;

            var request = await DoPostRequest("api/project/editprojectdata", newProject, HttpMethod.Patch);

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectDataForDropdown_ThereIsProjectDataToReturn_ShouldReturnData()
        {
            var request = await DoGetRequest("api/project/getallprojectsfordropdown", "");
            request.EnsureSuccessStatusCode();

            var result = GetMultipleResults<ProjectDataForDropdownDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectDataForDropdown_ThereIsProjectDataToReturn_ShouldReturnNoContent()
        {
            dbContext.PersonInProject.RemoveRange(dbContext.PersonInProject.ToList());
            dbContext.Project.RemoveRange(dbContext.Project.ToList());
            dbContext.SaveChanges();

            var request = await DoGetRequest("api/project/getallprojectsfordropdown", "");
            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task ExportProjectAsXlsx_TheProjectIdIsValidAndContainsDataToExport_ShouldReturnTheFileContent()
        {
            var request = await DoGetRequest("api/project/ExportProjectAsXlsx", $"projectId={newProject.ProjectId}");
            request.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.NotNull(await request.Content.ReadAsStringAsync());
            Assert.IsTrue(request.Content.Headers.ContentType.MediaType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Test]
        public async System.Threading.Tasks.Task ExportProjectAsXlsx_TheProjectIdIsValidButThereIsNoContent_ShouldReturnNoContent()
        {
            var request = await DoGetRequest("api/project/ExportProjectAsXlsx", $"projectId={Guid.NewGuid()}");

            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task ExportProjectAsXlsx_TheProjectIdIsInvalid_ShouldReturnBadRequest()
        {
            var request = await DoGetRequest("api/project/ExportProjectAsXlsx", $"projectId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task ExportProjectAsPdf_TheProjectIdIsValidAndContainsDataToExport_ShouldReturnTheFileContent()
        {
            var request = await DoGetRequest("api/project/exportprojectaspdf", $"projectId={newProject.ProjectId}");
            request.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.NotNull(await request.Content.ReadAsStringAsync());
            Assert.IsTrue(request.Content.Headers.ContentType.MediaType == "application/pdf");
        }

        [Test]
        public async System.Threading.Tasks.Task ExportProjectAsPdf_TheProjectIdIsValidButThereIsNoContent_ShouldReturnNoContent()
        {
            var request = await DoGetRequest("api/project/exportprojectaspdf", $"projectId={Guid.NewGuid()}");

            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task ExportProjectAsPdf_TheProjectIdIsInvalid_ShouldReturnBadRequest()
        {
            var request = await DoGetRequest("api/project/exportprojectaspdf", $"projectId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }
    }
}
