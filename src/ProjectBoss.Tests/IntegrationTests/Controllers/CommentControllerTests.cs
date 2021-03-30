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
    public class CommentControllerTests : HttpRequestIntegrationTestsBase
    {
        CreateTaskDto newTask;
        NewCommentDto taskComment;
        

        [OneTimeSetUp]
        public async Task Setup()
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

            await dbContext.Task.AddAsync(mapper.Map<Core.Entities.Task>(newTask));
            await dbContext.SaveChangesAsync();

            List<NewCommentDto> newComments = new List<NewCommentDto>()
            {
                new NewCommentDto {
                    TaskId = newTask.TaskId,
                    Content = "blabla",
                    PersonId = ADMIN_PERSON_ID
                },
                new NewCommentDto
                {
                    TaskId = newTask.TaskId,
                    Content = "blabla2",
                    PersonId = ADMIN_PERSON_ID
                }
            };

            foreach (var comment in newComments)
                await dbContext.Comment.AddAsync(mapper.Map<Core.Entities.Comment>(comment));

            await dbContext.SaveChangesAsync();
        }

        [Test]
        public async Task NewComment_TaskExistsAndNewCommentDataIsValid_ShouldSaveNewComment()
        {
            taskComment = new NewCommentDto
            {
                TaskId = newTask.TaskId,
                Content = "blabla",
                PersonId = ADMIN_PERSON_ID
            };

            var response = await DoPostRequest("api/comment/newcomment", taskComment);
            response.EnsureSuccessStatusCode();

            var result = GetSingleResult<NewCommentDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.AreEqual(result.CommentId, taskComment.CommentId);
        }

        [Test]
        public async Task NewComment_TaskExistsAndNewCommentDataInvalid_ShouldReturnBadRequest()
        {
            taskComment = new NewCommentDto
            {
                TaskId = newTask.TaskId                
            };

            var response = await DoPostRequest("api/comment/newcomment", taskComment);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);            
        }

        [Test]
        public async Task GetTaskComments_TaskExistsAndContainsComments_ShouldGetTaskComments()
        {
            var response = await DoGetRequest("api/comment/gettaskcomments", $"taskId={newTask.TaskId}");
            response.EnsureSuccessStatusCode();

            var result = GetMultipleResults<CommentDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public async Task GetTaskComments_TaskIdIsNull_ShouldReturnBadRequest()
        {
            var response = await DoGetRequest("api/comment/gettaskcomments", $"taskId={Guid.Empty}");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task GetTaskComments_TaskDontExist_ShouldReturnNoComments()
        {
            var response = await DoGetRequest("api/comment/gettaskcomments", $"taskId={Guid.NewGuid()}");
            response.EnsureSuccessStatusCode();

            var result = GetMultipleResults<CommentDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(result.Count == 0);
        }
    }
}
