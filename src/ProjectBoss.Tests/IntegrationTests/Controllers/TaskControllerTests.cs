using NUnit.Framework;
using ProjectBoss.Api.Controllers;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Tests.IntegrationTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.IntegrationTests.Controllers
{
    [TestFixture]
    public class TaskControllerTests : HttpRequestIntegrationTestsBase
    {
        CreateTaskDto newTask;
        
        public TaskControllerTests()
        {
            newTask = new CreateTaskDto
            {
                AttendantId = ADMIN_PERSON_ID,
                AuthorId = ADMIN_PERSON_ID,
                Title = "new task",
                Description = "new task",
                StatusId = 1,
                PriorityId = 1
            };
        }

        [OneTimeSetUp]
        public async Task SetupTests()
        {
            var eTask = newTask;
            eTask.Title = "Testing Task";

            dbContext.Task.Add(mapper.Map<ProjectBoss.Core.Entities.Task>(newTask));
            await dbContext.SaveChangesAsync();
        }

        [Test]
        public async Task CreateNewTask_GivenAnValidTaskData_ShouldCreateNewTask()
        {
            newTask = new CreateTaskDto
            {
                AttendantId = ADMIN_PERSON_ID,
                AuthorId = ADMIN_PERSON_ID,
                Title = "new task",
                Description = "new task",
                StatusId = 1,
                PriorityId = 1
            };

            var response = await DoPostRequest("api/task/newtask", newTask);
            response.EnsureSuccessStatusCode();

            var result = GetSingleResult<CreateTaskDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
        }

        [Test]
        public async Task CreateNewTask_GivenAnInvalidTaskData_ShouldReturnBadRequest()
        {
            newTask = new CreateTaskDto
            {
                AttendantId = ADMIN_PERSON_ID,
                AuthorId = ADMIN_PERSON_ID,                
                StatusId = 1,
                PriorityId = 1
            };

            var response = await DoPostRequest("api/task/newtask", newTask);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task GetAllActiveTasks_GivenThatDatabaseContainsTasks_ShouldReturnAllTasksFromDb()
        {
            var response = await DoGetRequest("api/task/getallactivetasks", "");
            response.EnsureSuccessStatusCode();

            var result = GetMultipleResults<TaskDto>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }

        [Test]
        public async Task GetTaskByAuthor_GivenThatTheUsedAuthorHasTask_ShouldReturnHisTasks()
        {
            var response = await DoGetRequest("api/task/gettasksbyauthor", $"authorId={ADMIN_PERSON_ID}");

            var result = GetMultipleResults<TaskDto>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
            Assert.True(result.First().AuthorId == ADMIN_PERSON_ID);
        }

        [Test]
        public async Task GetTaskByAuthor_GivenThatTheUsedAuthorDontExist_ShouldReturnNoTasks()
        {
            var response = await DoGetRequest("api/task/gettasksbyauthor", $"authorId={Guid.NewGuid()}");
            response.EnsureSuccessStatusCode();

            var result = GetMultipleResults<TaskDto>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.True(result.Count == 0);
        }

        [Test]
        public async Task GetTaskByAuthor_GivenThatTheAuthorIdIsGuidEmpty_ShouldReturnBadRequest()
        {
            var response = await DoGetRequest("api/task/gettasksbyauthor", $"authorId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task GetTasksByAttendant_GivenThatTheAttendantHasTask_ShouldReturnHisTasks()
        {
            var response = await DoGetRequest("api/task/gettasksbyattendant", $"attendantId={ADMIN_PERSON_ID}");

            var result = GetMultipleResults<TaskDto>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
            Assert.True(result.First().AttendantId == ADMIN_PERSON_ID);
        }

        [Test]
        public async Task GetTasksByAttendant_GivenThatTheAttendantDontExist_ShouldReturnNoTasks()
        {
            var response = await DoGetRequest("api/task/gettasksbyattendant", $"attendantId={Guid.NewGuid()}");
            response.EnsureSuccessStatusCode();

            var result = GetMultipleResults<TaskDto>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Count == 0);
        }

        [Test]
        public async Task GetTasksByAttendant_GivenThatTheAttendantIdIsGuidEmpty_ShouldReturnBadRequest()
        {
            var response = await DoGetRequest("api/task/gettasksbyattendant", $"attendantId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task GetTaskByProjectId_GivenThatProjectDoesNotExist_ShouldNotReturnAnyData()
        {
            var response = await DoGetRequest("api/task/gettasksbyprojectid", $"projectId={Guid.NewGuid()}");

            var result = GetMultipleResults<TaskDto>(await response.Content.ReadAsStringAsync());

            Assert.True(result.Count == 0);
        }

        [Test]
        public async Task GetTaskByProjectId_GivenThatProjectIdIsGuidEmpty_ShouldReturnBadRequest()
        {
            var response = await DoGetRequest("api/task/gettasksbyprojectid", $"projectId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task EditTask_GivenThatEditedTaskIsValid_ShouldEditTaskWithSuccess()
        {
            var getTaskRequest = await DoGetRequest("api/task/getallactivetasks", "");
            var getTaskResponse = GetMultipleResults<TaskDto>(await getTaskRequest.Content.ReadAsStringAsync());

            var editedTask = mapper.Map<EditTaskDto>(getTaskResponse.First());
            editedTask.Title = "Edited Task";

            var editTaskRequest = await DoPostRequest("api/task/EditTask", editedTask, HttpMethod.Patch);
            editTaskRequest.EnsureSuccessStatusCode();

            var editTaskResponse = GetStructResult<bool>(await editTaskRequest.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, editTaskRequest.StatusCode);
            Assert.IsTrue(editTaskResponse);
        }

        [Test]
        public async Task EditTask_GivenThatEditedTaskIsInvalid_ShouldReturnBadRequest()
        {
            var getTaskRequest = await DoGetRequest("api/task/getallactivetasks", "");
            var getTaskResponse = GetMultipleResults<TaskDto>(await getTaskRequest.Content.ReadAsStringAsync());

            var editedTask = mapper.Map<EditTaskDto>(getTaskResponse.First());
            editedTask.Title = string.Empty;
            editedTask.Description = string.Empty;

            var editTaskRequest = await DoPostRequest("api/task/EditTask", editedTask, HttpMethod.Patch);

            Assert.AreEqual(HttpStatusCode.BadRequest, editTaskRequest.StatusCode);
        }

        [Test]
        public async Task SetTaskComplete_GivenThatTaskExistsAndIsNotCompleted_ShouldSetTaskCompleteWithSuccess()
        {
            var getTaskRequest = await DoGetRequest("api/task/getallactivetasks", "");
            var getTaskResponse = GetMultipleResults<TaskDto>(await getTaskRequest.Content.ReadAsStringAsync());

            var setTaskCompleteRequest = await DoPostRequest("api/task/settaskcomplete", getTaskResponse.First().TaskId);
            var setTaskCompleteResponse = GetStructResult<bool>(await setTaskCompleteRequest.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, setTaskCompleteRequest.StatusCode);
            Assert.IsTrue(setTaskCompleteResponse);
        }

        [Test]
        public async Task SetTaskComplete_GivenThatTaskDontExists_ShouldReturnFalse()
        {
            var setTaskCompleteRequest = await DoPostRequest("api/task/settaskcomplete", Guid.NewGuid());
            var setTaskCompleteResponse = GetStructResult<bool>(await setTaskCompleteRequest.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, setTaskCompleteRequest.StatusCode);
            Assert.IsFalse(setTaskCompleteResponse);
        }

        [Test]
        public async Task SetTaskComplete_GivenThatTaskIdIsGuidEmpty_ShouldReturnBadRequest()
        {
            var setTaskCompleteRequest = await DoPostRequest("api/task/settaskcomplete", Guid.Empty);

            Assert.AreEqual(HttpStatusCode.BadRequest, setTaskCompleteRequest.StatusCode);
        }

        [Test]
        public async Task ToggleTaskStatus_GivenAnExistingTask_ShouldChangeTaskStatusWithSuccess()
        {
            var getTaskRequest = await DoGetRequest("api/task/getallactivetasks", "");
            var getTaskResponse = GetMultipleResults<TaskDto>(await getTaskRequest.Content.ReadAsStringAsync());

            var toggleTaskStatusRequest = await DoPostRequest("api/task/toggletaskstatus", new ToggleTaskStatusDto { StatusId = 2, TaskId = getTaskResponse.First().TaskId });
            var toggleTaskStatusResponse = GetStructResult<bool>(await toggleTaskStatusRequest.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, toggleTaskStatusRequest.StatusCode);
            Assert.IsTrue(toggleTaskStatusResponse);
        }

        [Test]
        public async Task ToggleTaskStatus_GivenAnUnexistingTask_ShouldReturnFalse()
        {
            var toggleTaskStatusRequest = await DoPostRequest("api/task/toggletaskstatus", new ToggleTaskStatusDto { StatusId = 2, TaskId = Guid.NewGuid() });
            var toggleTaskStatusResponse = GetStructResult<bool>(await toggleTaskStatusRequest.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, toggleTaskStatusRequest.StatusCode);
            Assert.IsFalse(toggleTaskStatusResponse);
        }

        [Test]
        public async Task ToggleTaskStatus_GivenAnUnexistingStatusId_ShouldReturnFalse()
        {
            var getTaskRequest = await DoGetRequest("api/task/getallactivetasks", "");
            var getTaskResponse = GetMultipleResults<TaskDto>(await getTaskRequest.Content.ReadAsStringAsync());

            var toggleTaskStatusRequest = await DoPostRequest("api/task/toggletaskstatus", new ToggleTaskStatusDto { StatusId = 22, TaskId = getTaskResponse.First().TaskId });
            var toggleTaskStatusResponse = GetStructResult<bool>(await toggleTaskStatusRequest.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, toggleTaskStatusRequest.StatusCode);
            Assert.IsFalse(toggleTaskStatusResponse);
        }

        [Test]
        public async Task RemoveTask_GivenAnExistingTask_ShouldRemoveTask()
        {
            var taskToRemove = new CreateTaskDto
            {
                AttendantId = ADMIN_PERSON_ID,
                AuthorId = ADMIN_PERSON_ID,
                Title = "new task",
                Description = "new task",
                StatusId = 1,
                PriorityId = 1
            };

            var newTaskReq = await DoPostRequest("api/task/newtask", taskToRemove);
            newTaskReq.EnsureSuccessStatusCode();

            var newTaskResp = GetSingleResult<CreateTaskDto>(await newTaskReq.Content.ReadAsStringAsync());

            var removeTaskReq = await DoPostRequest("api/task/removetask", newTaskResp.TaskId);
            removeTaskReq.EnsureSuccessStatusCode();

            var removeTaskResp = GetStructResult<bool>(await removeTaskReq.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, removeTaskReq.StatusCode);
            Assert.IsTrue(removeTaskResp);
        }

        [Test]
        public async Task RemoveTask_GivenThatTheTaskIDIsGuidEmpty_ShouldReturnBadRequest()
        {
            var removeTaskReq = await DoPostRequest("api/task/removetask", Guid.Empty);            

            Assert.AreEqual(HttpStatusCode.BadRequest, removeTaskReq.StatusCode);
        }

        [Test]
        public async Task ExportTasksAsXlsx_GivenAnExistingUserWithTasks_ShouldReturnTheFileToDownload()
        {
            var getFileReq = await DoGetRequest("api/task/exporttasksasxlsx", $"userId={ADMIN_PERSON_ID}");
            getFileReq.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, getFileReq.StatusCode);
            Assert.NotNull(await getFileReq.Content.ReadAsStringAsync());
            Assert.IsTrue(getFileReq.Content.Headers.ContentType.MediaType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Test]
        public async Task ExportTasksAsXlsx_GivenThatTaskIdIsGuidEmpty_ShouldReturnBadRequest()
        {
            var getFileReq = await DoGetRequest("api/task/exporttasksasxlsx", $"{Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, getFileReq.StatusCode);
        }

        [Test]
        public async Task ExportTasksAsPdf_GivenAnExistingUserIdWithTasks_ShouldReturnTheFileToDownload()
        {
            var getFileReq = await DoGetRequest("api/task/exporttasksaspdf", $"userId={ADMIN_PERSON_ID}");
            getFileReq.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, getFileReq.StatusCode);
            Assert.NotNull(await getFileReq.Content.ReadAsStringAsync());
            Assert.IsTrue(getFileReq.Content.Headers.ContentType.MediaType == "application/pdf");
        }

        [Test]
        public async Task ExportTasksAsPdf_GivenThatTaskIdIsGuidEmpty_ShouldReturnBadRequest()
        {
            var getFileReq = await DoGetRequest("api/task/exporttasksaspdf", $"{Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, getFileReq.StatusCode);            
        }
    }
}
