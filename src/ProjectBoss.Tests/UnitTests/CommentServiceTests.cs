using AutoMapper;
using Moq;
using NUnit.Framework;
using ProjectBoss.Api.Configuration;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.UnitTests
{
    [TestFixture]
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> mockCommentRepository;
        private readonly IMapper mapper;

        private Guid mockTaskId = Guid.Parse("90D26708-694B-4630-8FDA-FB4CD98B06D0");

        public CommentServiceTests()
        {
            mockCommentRepository = new Mock<ICommentRepository>();
            mapper = new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper();
        }

        [TearDown]
        public async Task AfterEachTest()
        {
            mockCommentRepository.Invocations.Clear();
        }

        [Test]
        public async Task GetCommentsByTask_ThereIsCommentsForAGivenTask_ShouldReturnItsComments()
        {
            mockCommentRepository.Setup(x => x.GetTaskComments(mockTaskId))
                                 .Returns(Task.FromResult(GetComments().Where(x => x.TaskId == mockTaskId)));

            var commentService = new CommentService(mockCommentRepository.Object, mapper);

            var result = await commentService.GetCommentsByTask(mockTaskId);

            Assert.IsTrue(result.Any());
            Assert.IsTrue(result.All(x => x.TaskId == mockTaskId));
        }

        [Test]
        public async Task GetCommentsByTask_ThereIsntCommentsForAGivenTask_ShouldReturnEmptyList()
        {
            mockCommentRepository.Setup(x => x.GetTaskComments(mockTaskId))
                                 .Returns(Task.FromResult<IEnumerable<Core.Entities.Comment>>(null));

            var commentService = new CommentService(mockCommentRepository.Object, mapper);

            var result = await commentService.GetCommentsByTask(mockTaskId);

            Assert.IsTrue(!result.Any());
        }

        [Test]
        public async Task NewComment_GivenAValidCommentDataAndSaveNewCommentSuccessfully_ShouldReturnCreatedComment()
        {
            var newComment = new NewCommentDto
            {
                Content = "New comment",
                PersonId = Guid.NewGuid(),
                TaskId = Guid.NewGuid()
            };

            mockCommentRepository.Setup(x => x.Create(mapper.Map<Core.Entities.Comment>(newComment)))
                                 .Returns(Task.CompletedTask);
            mockCommentRepository.Setup(x => x.SaveChanges())
                                 .ReturnsAsync(true);

            var commentService = new CommentService(mockCommentRepository.Object, mapper);

            var result = await commentService.NewComment(newComment);

            mockCommentRepository.Verify(v => v.SaveChanges(), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(newComment.CommentId, result.CommentId);
            Assert.AreEqual(newComment.Content, result.Content);
        }

        [Test]
        public async Task NewComment_GivenAValidCommentDataAndShouldFailToSave_ShouldReturnNull()
        {
            var newComment = new NewCommentDto
            {
                Content = "New comment",
                PersonId = Guid.NewGuid(),
                TaskId = Guid.NewGuid()
            };

            mockCommentRepository.Setup(x => x.Create(mapper.Map<Core.Entities.Comment>(newComment)))
                                 .Returns(Task.CompletedTask);
            mockCommentRepository.Setup(x => x.SaveChanges())
                                 .ReturnsAsync(false);

            var commentService = new CommentService(mockCommentRepository.Object, mapper);

            var result = await commentService.NewComment(newComment);

            mockCommentRepository.Verify(v => v.SaveChanges(), Times.Once);

            Assert.IsNull(result);
        }

        public IEnumerable<Core.Entities.Comment> GetComments()
        {
            List<Core.Entities.Comment> comments = new List<Core.Entities.Comment>();

            comments.Add(new Core.Entities.Comment
            {
                CommentId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                PersonId = Guid.NewGuid(),
                TaskId = mockTaskId,
                Content = "Comment 1"
            });

            comments.Add(new Core.Entities.Comment
            {
                CommentId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                PersonId = Guid.NewGuid(),
                TaskId = mockTaskId,
                Content = "Comment 2"
            });

            comments.Add(new Core.Entities.Comment
            {
                CommentId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                PersonId = Guid.NewGuid(),
                TaskId = mockTaskId,
                Content = "Comment 3"
            });

            comments.Add(new Core.Entities.Comment
            {
                CommentId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                PersonId = Guid.NewGuid(),
                TaskId = Guid.NewGuid(),
                Content = "Comment 4"
            });

            comments.Add(new Core.Entities.Comment
            {
                CommentId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                PersonId = Guid.NewGuid(),
                TaskId = Guid.NewGuid(),
                Content = "Comment 5"
            });

            return comments;
        }
    }
}
