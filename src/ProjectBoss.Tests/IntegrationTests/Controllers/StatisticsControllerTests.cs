using Moq;
using NUnit.Framework;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services.Interfaces;
using ProjectBoss.Core.Entities;
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
    public class StatisticsControllerTests : HttpRequestIntegrationTestsBase
    {
        ProjectDto testProject;
        PersonBasicDto taskAttendant;
        CreateProjectTaskDto projectTask;

        public StatisticsControllerTests()
        {
            taskAttendant = mapper.Map<PersonBasicDto>(dbContext.Person.ToList().FirstOrDefault());
            projectTask = new CreateProjectTaskDto
            {
                Attendant = taskAttendant,
                AuthorId = ADMIN_PERSON_ID,
                StatusId = 1,
                PriorityId = 1,
                Title = "Task 1",
                Description = "Task 1",
                ConcludedDate = DateTime.Now
            };

            testProject = new ProjectDto
            {
                AuthorId = ADMIN_PERSON_ID,
                Title = "New Project",
                Description = "New Project",
                StartDate = DateTime.Now.Date,
                ConclusionDate = DateTime.Now.AddMonths(1),
                ConcludedDate = DateTime.Now,
                AttendantIds = new List<Guid> { ADMIN_PERSON_ID },
                Tasks = new List<CreateProjectTaskDto>
                {
                    projectTask
                }
            };
        }

        [SetUp]
        public async System.Threading.Tasks.Task Setup()
        {
            if (!dbContext.Project.ToList().Any())
            {
                dbContext.Project.Add(mapper.Map<Project>(testProject));

                foreach (var attendantId in testProject.AttendantIds)
                    dbContext.PersonInProject.Add(new Domain.Entities.PersonInProject { PersonId = attendantId, ProjectId = testProject.ProjectId });

                await dbContext.SaveChangesAsync();
            }            
        }

        [Test]
        public async System.Threading.Tasks.Task GetPersonOverviewStatistics_GivenAnExistingPersonId_ShouldReturnOkResult()
        {
            var request = await DoGetRequest("api/statistics/getpersonoverviewstatistics", $"personId={ADMIN_PERSON_ID}");
            request.EnsureSuccessStatusCode();

            var result = GetSingleResult<PersonOverviewStatsDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.NotNull(result);
        }

        [Test]
        public async System.Threading.Tasks.Task GetPersonOverviewStatistics_GivenAnInvalidPersonId_ShouldReturnBadRequest()
        {
            var request = await DoGetRequest("api/statistics/getpersonoverviewstatistics", $"personId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetOpenAndOnGoingTasksByPersonInProject_GivenAnExistingProjectId_ShouldReturnOkResult()
        {
            var request = await DoGetRequest("api/statistics/GetOpenAndOnGoingTasksByPersonInProject", $"projectId={testProject.ProjectId}");
            request.EnsureSuccessStatusCode();

            var result = GetMultipleResults<PersonOverviewStatsDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetOpenAndOnGoingTasksByPersonInProject_GivenAnUnexistingProjectId_ShouldReturnNoContent()
        {
            var request = await DoGetRequest("api/statistics/GetOpenAndOnGoingTasksByPersonInProject", $"projectId={Guid.NewGuid()}");

            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetOpenAndOnGoingTasksByPersonInProject_GivenAnInvalidProjectId_ShouldReturnBadRequest()
        {
            var request = await DoGetRequest("api/statistics/GetOpenAndOnGoingTasksByPersonInProject", $"projectId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTasksStatusByProject_GivenAnExistingProjectId_ShouldReturnOkResult()
        {
            var request = await DoGetRequest("api/statistics/GetTasksStatusByProject", $"projectId={testProject.ProjectId}");
            request.EnsureSuccessStatusCode();

            var result = GetMultipleResults<PersonOverviewStatsDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTasksStatusByProject_GivenAnUnexistingProjectId_ShouldReturnNoContent()
        {
            var request = await DoGetRequest("api/statistics/GetTasksStatusByProject", $"projectId={Guid.NewGuid()}");

            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTasksStatusByProject_GivenAnInvalidProjectId_ShouldReturnBadRequest()
        {
            var request = await DoGetRequest("api/statistics/GetTasksStatusByProject", $"projectId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetNewAndClosedTasksByDateByProject_GivenAnExistingProjectId_ShouldReturnOkResult()
        {
            var request = await DoGetRequest("api/statistics/GetNewAndClosedTasksByDateByProject", $"projectId={testProject.ProjectId}");
            request.EnsureSuccessStatusCode();

            var result = GetMultipleResults<PersonOverviewStatsDto>(await request.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetNewAndClosedTasksByDateByProject_GivenAnUnexistingProjectId_ShouldReturnNoContent()
        {
            var request = await DoGetRequest("api/statistics/GetNewAndClosedTasksByDateByProject", $"projectId={Guid.NewGuid()}");

            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetNewAndClosedTasksByDateByProject_GivenAnInvalidProjectId_ShouldReturnBadRequest()
        {
            var request = await DoGetRequest("api/statistics/GetNewAndClosedTasksByDateByProject", $"projectId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetCreatedUsers_GivenThatThereIsCreatedUsers_ShouldReturnOkResult()
        {
            var request = await DoGetRequest("api/statistics/GetCreatedUsers", "");
            request.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTotalCreatedTasksByDate_GivenThatThereIsTasksCreated_ShouldReturnOkResult()
        {
            if (!dbContext.Task.Any())
            {
                dbContext.Task.Add(mapper.Map<Core.Entities.Task>(projectTask));
                dbContext.SaveChanges();
            }

            var request = await DoGetRequest("api/statistics/GetTotalCreatedTasksByDate", "");
            request.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTotalCreatedTasksByDate_GivenThatThereIsNoTasksCreated_ShouldReturnNoContent()
        {
            dbContext.Task.RemoveRange(dbContext.Task.ToList());
            dbContext.SaveChanges();

            var request = await DoGetRequest("api/statistics/GetTotalCreatedTasksByDate", "");
            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTotalConcludedTasksByDate_GivenThatThereIsTasksConcluded_ShouldReturnOkResult()
        {
            if (!dbContext.Task.Any())
            {
                dbContext.Task.Add(mapper.Map<Core.Entities.Task>(projectTask));
                dbContext.SaveChanges();
            }

            var request = await DoGetRequest("api/statistics/GetTotalConcludedTasksByDate", "");
            request.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTotalConcludedTasksByDate_GivenThatThereIsNoTasksCreated_ShouldReturnNoContent()
        {
            dbContext.Task.RemoveRange(dbContext.Task.ToList());
            dbContext.SaveChanges();

            var request = await DoGetRequest("api/statistics/GetTotalConcludedTasksByDate", "");
            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTotalCreatedProjectsByDate_GivenThatThereIsProjectsCreated_ShouldReturnOkResult()
        {
            var request = await DoGetRequest("api/statistics/GetTotalCreatedProjectsByDate", "");
            request.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTotalCreatedProjectsByDate_GivenThatThereIsNoProjectsCreated_ShouldReturnNoContent()
        {
            dbContext.Task.RemoveRange(dbContext.Task.ToList());
            dbContext.PersonInProject.RemoveRange(dbContext.PersonInProject.ToList());
            dbContext.Project.RemoveRange(dbContext.Project.ToList());
            dbContext.SaveChanges();

            var request = await DoGetRequest("api/statistics/GetTotalCreatedProjectsByDate", "");
            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTotalConcludedProjectsByDate_GivenThatThereIsConcludedProjects_ShouldReturnOkResult()
        {
            var request = await DoGetRequest("api/statistics/GetTotalConcludedProjectsByDate", "");
            request.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTotalConcludedProjectsByDate_GivenThatThereIsNoConcludedProjects_ShouldReturnNoContent()
        {
            dbContext.Task.RemoveRange(dbContext.Task.ToList());
            dbContext.PersonInProject.RemoveRange(dbContext.PersonInProject.ToList());
            dbContext.Project.RemoveRange(dbContext.Project.ToList());
            dbContext.SaveChanges();

            var request = await DoGetRequest("api/statistics/GetTotalConcludedProjectsByDate", "");
            Assert.AreEqual(HttpStatusCode.NoContent, request.StatusCode);
        }
    }
}
