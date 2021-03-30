using AutoFixture;
using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Moq;
using NUnit.Framework;
using ProjectBoss.Api.Configuration;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.UnitTests
{
    [TestFixture]
    public class TaskServiceUnitTests
    {
        private readonly Mock<ITaskRepository> taskRepositoryMock;
        private readonly IMapper mapper;
        private IConverter converter;

        Guid mockAuthorId = Guid.Parse("C7ED37F2-D609-4CBB-B85B-277C9A255A37");
        Guid mockAttendantId = Guid.Parse("6115B132-51CE-42E6-AA84-E35B9D2142C7");
        Guid mockProjectId = Guid.Parse("CE9E3632-6AB4-42E6-A543-9B676559C712");
        Guid mockTaskId = Guid.Parse("62C0CAB6-D616-4C64-A483-4DC25B3995EC");

        public TaskServiceUnitTests()
        {
            converter = new SynchronizedConverter(new PdfTools());
            taskRepositoryMock = new Mock<ITaskRepository>();

            mapper = new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper();
        }

        [Test]
        public async Task CreateNewTask_GivenValidTaskData_ShouldCreateNewTask()
        {
            var task = new CreateTaskDto
            {
                AttendantId = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                StatusId = 1,
                PriorityId = 1,
                Title = "Test Task",
                Description = "Task Test"
            };

            taskRepositoryMock.Setup(s => s.Create(mapper.Map<Core.Entities.Task>(task))).Returns(Task.FromResult(task));
            taskRepositoryMock.Setup(s => s.SaveChanges()).Returns(Task.FromResult(true));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.CreateNewTask(task);

            taskRepositoryMock.Verify(v => v.SaveChanges(), Times.AtLeastOnce);

            Assert.NotNull(response);
            Assert.AreEqual(response, task);
        }

        [Test]
        public async Task EditTask_GivenUnexistentTaskToEdit_ShouldReturnNull()
        {
            var task = new EditTaskDto
            {
                AttendantId = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                StatusId = 1,
                PriorityId = 1,
                Title = "Test Task",
                Description = "Task Test"
            };

            taskRepositoryMock.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Task, bool>>>()))
                              .Returns(Task.FromResult<Core.Entities.Task>(null));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.EditTask(task);

            Assert.IsFalse(response);
        }

        [Test]
        public async Task EditTask_GivenAnExistentTaskToEdit_ShouldEditTask()
        {
            var taskId = Guid.NewGuid();
            var personId = Guid.NewGuid();

            var taskEntity = new Core.Entities.Task
            {
                TaskId = taskId,
                StatusId = 1,
                PriorityId = 1,
                Title = "Test Task",
                Description = "Task Test",
                AttendantId = personId,
                AuthorId = personId,
                CreatedDate = DateTime.Now
            };
            var editedTask = new EditTaskDto
            {
                TaskId = taskId,
                AttendantId = personId,
                AuthorId = personId,
                StatusId = 1,
                PriorityId = 1,
                Title = "Test Task edit",
                Description = "Task Test edit"
            };

            taskRepositoryMock.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Task, bool>>>()))
                              .Returns(Task.FromResult(taskEntity));

            taskRepositoryMock.Setup(s => s.Update(It.IsAny<Core.Entities.Task>()))
                              .Returns(Task.CompletedTask);

            taskRepositoryMock.Setup(s => s.SaveChanges())
                              .Returns(Task.FromResult(true));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.EditTask(editedTask);

            taskRepositoryMock.Verify(v => v.SaveChanges(), Times.AtLeastOnce);

            Assert.IsTrue(response);
        }

        [Test]
        public async Task GetTaskById_GivenAnExistingTaskId_ShouldReturnTask()
        {
            var taskId = Guid.NewGuid();
            var taskEntity = new Core.Entities.Task
            {
                TaskId = taskId,
                StatusId = 1,
                PriorityId = 1,
                Title = "Test Task",
                Description = "Task Test",
                AttendantId = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                CreatedDate = DateTime.Now
            };

            taskRepositoryMock.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Task, bool>>>()))
                              .Returns(Task.FromResult(taskEntity));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.GetTaskByTaskId(It.IsAny<Guid>());

            Assert.NotNull(response);
            Assert.AreEqual(taskId, response.TaskId);
        }

        [Test]
        public async Task GetTaskById_GivenAnUnexistingTaskId_ShouldNull()
        {
            taskRepositoryMock.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Task, bool>>>()))
                              .Returns(Task.FromResult<Core.Entities.Task>(null));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.GetTaskByTaskId(It.IsAny<Guid>());

            Assert.IsNull(response);
        }

        [Test]
        public async Task GetTasksByAttendant_GivenAValidAndExistentAttendantId_ShouldReturnTask()
        {
            var taskId = Guid.NewGuid();
            var attendantId = Guid.NewGuid();
            var taskEntity = new Core.Entities.Task
            {
                TaskId = taskId,
                StatusId = 1,
                PriorityId = 1,
                Title = "Test Task",
                Description = "Task Test",
                AttendantId = attendantId,
                AuthorId = Guid.NewGuid(),
                CreatedDate = DateTime.Now
            };

            List<Core.Entities.Task> tasks = new List<Core.Entities.Task>() { taskEntity };

            taskRepositoryMock.Setup(s => s.GetTasksByAttendant(It.IsAny<Guid>()))
                              .Returns(Task.FromResult(tasks));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.GetTasksByAttendant(It.IsAny<Guid>());

            Assert.NotNull(response);
            Assert.True(response.Count > 0);
        }

        [Test]
        public async Task GetTasksByAttendant_GivenAnUnexistentAttendantId_ShouldReturnNoTasks()
        {
            var taskId = Guid.NewGuid();
            var attendantId = Guid.NewGuid();
            var taskEntity = new Core.Entities.Task
            {
                TaskId = taskId,
                StatusId = 1,
                PriorityId = 1,
                Title = "Test Task",
                Description = "Task Test",
                AttendantId = attendantId,
                AuthorId = Guid.NewGuid(),
                CreatedDate = DateTime.Now
            };

            List<Core.Entities.Task> tasks = new List<Core.Entities.Task>() { taskEntity };

            taskRepositoryMock.Setup(s => s.GetTasksByAttendant(It.IsAny<Guid>()))
                              .Returns(Task.FromResult(new List<Core.Entities.Task>()));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.GetTasksByAttendant(It.IsAny<Guid>());

            Assert.NotNull(response);
            Assert.True(response.Count == 0);
        }

        [Test]
        public async Task GetAllActiveTasks_GivenThatThereIsActiveTasks_ShouldReturnActiveTasks()
        {
            taskRepositoryMock.Setup(s => s.GetAllActiveTasksWithChildEntities())
                              .Returns(Task.FromResult(GetTasks().Where(x => !x.Removed).ToList()));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.GetAllActiveTasks();

            Assert.IsTrue(response.Any());
            Assert.IsTrue(response.Count == 3);
        }

        [Test]
        public async Task GetAllActiveTasks_GivenThatThereIsntActiveTasks_ShouldReturnEmptyList()
        {
            taskRepositoryMock.Setup(s => s.GetAllActiveTasksWithChildEntities())
                              .Returns(Task.FromResult(new List<Core.Entities.Task>()));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.GetAllActiveTasks();

            Assert.IsTrue(!response.Any());
        }

        [Test]
        public async Task GetTasksByAuthorId_GivenThatTheAuthorHasTasks_ShouldReturnHisTasks()
        {
            var authorId = mockAuthorId;

            taskRepositoryMock.Setup(s => s.GetTasksByAuthor(It.IsAny<Guid>()))
                              .Returns(Task.FromResult(GetTasks().Where(x => x.AuthorId == authorId).ToList()));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.GetTasksByAuthorId(authorId);

            Assert.IsTrue(response.Any());
            Assert.IsTrue(response.All(x => x.AuthorId == authorId));
        }

        [Test]
        public async Task GetTasksByAuthorId_GivenThatTheAuthorHasNoTasks_ShouldReturnEmptyList()
        {
            var authorId = Guid.NewGuid();

            taskRepositoryMock.Setup(s => s.GetTasksByAuthor(It.IsAny<Guid>()))
                              .Returns(Task.FromResult(new List<Core.Entities.Task>()));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.GetTasksByAuthorId(authorId);

            Assert.IsTrue(!response.Any());
        }

        [Test]
        public async Task GetTasksByProjectId_GivenThatTheProjectHasTasks_ShouldReturnListOfTasks()
        {
            var projectId = mockProjectId;

            taskRepositoryMock.Setup(s => s.GetTasksByProjectIdWithChildEntities(It.IsAny<Guid>()))
                              .Returns(
                                Task.FromResult(GetTasks().Where(x => x.ProjectId == projectId).ToList())
                              );

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.GetTasksByProjectId(projectId);

            Assert.IsTrue(response.Any());
            Assert.IsTrue(response.All(x => x.ProjectId == projectId));
        }

        [Test]
        public async Task GetTasksByProjectId_GivenThatTheProjectHasNoTasks_ShouldReturnEmptyList()
        {
            var projectId = mockProjectId;

            taskRepositoryMock.Setup(s => s.GetTasksByProjectIdWithChildEntities(It.IsAny<Guid>()))
                              .Returns(
                                Task.FromResult(new List<Core.Entities.Task>())
                              );

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.GetTasksByProjectId(projectId);

            Assert.IsTrue(!response.Any());
        }

        [Test]
        public async Task RemoveTask_GivenThatTheTaskExists_ShouldRemoveTheTask()
        {
            var taskId = mockTaskId;

            taskRepositoryMock.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Task, bool>>>()))
                              .Returns(
                                Task.FromResult(GetTasks().Where(x => x.TaskId == taskId).FirstOrDefault())
                              );
            taskRepositoryMock.Setup(s => s.Update(It.IsAny<Core.Entities.Task>()))
                              .Returns(Task.CompletedTask);

            taskRepositoryMock.Setup(s => s.SaveChanges())
                              .Returns(Task.FromResult(true));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.RemoveTask(taskId);

            taskRepositoryMock.Verify(v => v.Update(It.IsAny<Core.Entities.Task>()), Times.AtLeastOnce);
            taskRepositoryMock.Verify(v => v.SaveChanges(), Times.AtLeastOnce);

            Assert.IsTrue(response);
        }

        [Test]
        public async Task RemoveTask_GivenThatTheTaskDontExists_ShouldntRemoveTheTaskAndReturnFalse()
        {
            var taskId = mockTaskId;

            taskRepositoryMock.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Task, bool>>>()))
                              .Returns(
                                Task.FromResult<Core.Entities.Task>(null)
                              );

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.RemoveTask(taskId);

            Assert.IsFalse(response);
        }

        [Test]
        public async Task SetTaskCompleted_GivenThatTheTaskExists_ShouldSetTheTaskComplete()
        {
            var taskId = mockTaskId;

            taskRepositoryMock.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Task, bool>>>()))
                              .Returns(
                                Task.FromResult(GetTasks().Where(x => x.TaskId == taskId).FirstOrDefault())
                              );

            taskRepositoryMock.Setup(x => x.Update(It.IsAny<Core.Entities.Task>())).Returns(Task.CompletedTask);
            taskRepositoryMock.Setup(x => x.SaveChanges()).Returns(Task.FromResult(true));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.SetTaskCompleted(taskId);

            Assert.IsTrue(response);
        }

        [Test]
        public async Task SetTaskCompleted_GivenThatTheTaskDontExist_ShouldReturnFalse()
        {
            var taskId = mockTaskId;

            taskRepositoryMock.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Task, bool>>>()))
                              .Returns(
                                Task.FromResult<Core.Entities.Task>(null)
                              );            

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);

            var response = await taskService.SetTaskCompleted(taskId);

            Assert.IsFalse(response);
        }

        [Test]
        public async Task ToggleTaskStatus_GivenAnExistingTaskAndAnExistingStatusId_ShouldChangeTaskStatus()
        {
            var taskId = mockTaskId;

            taskRepositoryMock.Setup(x => x.GetTaskByTaskIdWithChildEntities(It.IsAny<Guid>()))
                              .Returns(
                                Task.FromResult(GetTasks().Where(x => x.TaskId == taskId).FirstOrDefault())
                              );

            taskRepositoryMock.Setup(x => x.Update(It.IsAny<Core.Entities.Task>())).Returns(Task.CompletedTask);
            taskRepositoryMock.Setup(x => x.SaveChanges()).Returns(Task.FromResult(true));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ToggleTaskStatus(taskId, 2);

            Assert.IsTrue(response);
        }

        [Test]
        public async Task ToggleTaskStatus_GivenAnUnexistingTaskAndAnExistingStatusId_ShouldNotChangeTaskStatus()
        {
            var taskId = mockTaskId;

            taskRepositoryMock.Setup(x => x.GetTaskByTaskIdWithChildEntities(It.IsAny<Guid>()))
                              .Returns(
                                Task.FromResult<Core.Entities.Task>(null)
                              );            

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ToggleTaskStatus(taskId, 2);

            Assert.IsFalse(response);
        }

        [Test]
        public async Task ToggleTaskStatus_GivenAnExistingTaskAndAnUnexistingStatusId_ShouldNotChangeTaskStatus()
        {
            var taskId = mockTaskId;

            taskRepositoryMock.Setup(x => x.GetTaskByTaskIdWithChildEntities(It.IsAny<Guid>()))
                              .Returns(
                                Task.FromResult(GetTasks().Where(x => x.TaskId == taskId).FirstOrDefault())
                              );

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ToggleTaskStatus(taskId, 45);

            Assert.IsFalse(response);
        }

        [Test]
        public async Task ToggleTaskStatus_GivenAnUnexistingTaskAndAnUnexistingStatusId_ShouldNotChangeTaskStatus()
        {
            var taskId = mockTaskId;

            taskRepositoryMock.Setup(x => x.GetTaskByTaskIdWithChildEntities(It.IsAny<Guid>()))
                              .Returns(
                                Task.FromResult<Core.Entities.Task>(null)
                              );

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ToggleTaskStatus(taskId, 45);

            Assert.IsFalse(response);
        }

        [Test]
        public async Task ExportTasksAsXlsl_GivenThatThereIsTasksAndIsUnrestricted_ShouldCreateTheByteArray()
        {
            taskRepositoryMock.Setup(x => x.GetAllTasksWithChildEntities())
                              .Returns(Task.FromResult(GetTasks()));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ExportTasksAsXlsl(It.IsAny<Guid>(), false);

            Assert.IsTrue(response.Length > 0);
        }

        [Test]
        public async Task ExportTasksAsXlsl_GivenThatThereIsTasksAndIsRestricted_ShouldCreateTheByteArray()
        {
            taskRepositoryMock.Setup(x => x.GetAllTasksByUserIdWithChildEntities(It.IsAny<string>()))
                              .Returns(Task.FromResult(GetTasks()));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ExportTasksAsXlsl(It.IsAny<Guid>(), true);

            Assert.IsTrue(response.Length > 0);
        }

        [Test]
        public async Task ExportTasksAsXlsl_GivenThatThereNoTasksAndIsRestricted_ShouldReturnNull()
        {
            taskRepositoryMock.Setup(x => x.GetAllTasksByUserIdWithChildEntities(It.IsAny<string>()))
                              .Returns(Task.FromResult<List<Core.Entities.Task>>(null));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ExportTasksAsXlsl(It.IsAny<Guid>(), true);

            Assert.IsNull(response);
        }

        [Test]
        public async Task ExportTasksAsXlsl_GivenThatThereNoTasksAndIsUnrestricted_ShouldReturnNull()
        {
            taskRepositoryMock.Setup(x => x.GetAllTasksWithChildEntities())
                              .Returns(Task.FromResult<List<Core.Entities.Task>>(null));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ExportTasksAsXlsl(It.IsAny<Guid>(), false);

            Assert.IsNull(response);
        }

        [Test]
        public async Task ExportTasksAsPdf_GivenThatThereIsTasksAndIsUnrestricted_ShouldCreateTheByteArray()
        {
            taskRepositoryMock.Setup(x => x.GetAllTasksWithChildEntities())
                              .Returns(Task.FromResult(GetTasks()));           

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ExportTasksAsPdf(It.IsAny<Guid>(), false);

            Assert.NotNull(response);
        }

        [Test]
        public async Task ExportTasksAsPdf_GivenThatThereIsTasksAndIsRestricted_ShouldCreateTheByteArray()
        {
            taskRepositoryMock.Setup(x => x.GetAllTasksByUserIdWithChildEntities(It.IsAny<string>()))
                              .Returns(Task.FromResult(GetTasks()));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ExportTasksAsPdf(It.IsAny<Guid>(), true);

            Assert.IsTrue(response.Length > 0);
        }

        [Test]
        public async Task ExportTasksAsPdf_GivenThatThereIsntTasksAndIsUnrestricted_ShouldReturnNull()
        {
            taskRepositoryMock.Setup(x => x.GetAllTasksWithChildEntities())
                              .Returns(Task.FromResult<List<Core.Entities.Task>>(null));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ExportTasksAsPdf(It.IsAny<Guid>(), false);

            Assert.IsNull(response);
        }

        [Test]
        public async Task ExportTasksAsPdf_GivenThatThereIsntTasksAndIsRestricted_ShouldReturnNull()
        {
            taskRepositoryMock.Setup(x => x.GetAllTasksByUserIdWithChildEntities(It.IsAny<string>()))
                              .Returns(Task.FromResult<List<Core.Entities.Task>>(null));

            var taskService = new TaskService(taskRepositoryMock.Object, mapper, converter);
            var response = await taskService.ExportTasksAsPdf(It.IsAny<Guid>(), true);

            Assert.IsNull(response);
        }

        public List<Core.Entities.Task> GetTasks()
        {
            Core.Entities.Person author = new Core.Entities.Person 
            { 
                UserId = Guid.NewGuid().ToString(), 
                PersonId = mockAuthorId,
                FirstName = "NewPerson",
                LastName = "NewPerson",
                CreatedDate = DateTime.Now
            };
            Core.Entities.Person attendant = new Core.Entities.Person
            {
                UserId = Guid.NewGuid().ToString(),
                PersonId = mockAttendantId,
                FirstName = "NewPerson",
                LastName = "NewPerson",
                CreatedDate = DateTime.Now
            };

            List<Core.Entities.Task> tasks = new List<Core.Entities.Task>();

            tasks.Add(new Core.Entities.Task
            {
                TaskId = mockTaskId,
                Attendant = attendant,
                Author = author,
                AuthorId = mockAuthorId,
                AttendantId = mockAttendantId,
                ProjectId = mockProjectId,
                Title = "Title",
                Description = "Description",
                StatusId = 1,
                PriorityId = 2,
                CreatedDate = DateTime.Now.AddDays(-5),
                ConclusionDate = DateTime.Now.AddDays(10)
            });
            tasks.Add(new Core.Entities.Task
            {
                TaskId = Guid.NewGuid(),
                Attendant = attendant,
                AuthorId = Guid.NewGuid(),
                Author = author,
                ProjectId = mockProjectId,
                Title = "Title",
                Description = "Description",
                StatusId = 2,
                PriorityId = 2,
                CreatedDate = DateTime.Now.AddDays(-5),
                ConclusionDate = DateTime.Now.AddDays(10),
                ConcludedDate = DateTime.Now
            });
            tasks.Add(new Core.Entities.Task
            {
                TaskId = Guid.NewGuid(),
                Attendant = attendant,
                AttendantId = mockAttendantId,
                Author = author,
                AuthorId = mockAuthorId,
                Title = "Title",
                Description = "Description",
                StatusId = 1,
                PriorityId = 2,
                CreatedDate = DateTime.Now.AddDays(-5),
                ConclusionDate = DateTime.Now.AddDays(10)
            });
            tasks.Add(new Core.Entities.Task
            {
                TaskId = Guid.NewGuid(),
                Attendant = attendant,
                AuthorId = Guid.NewGuid(),
                Author = author,
                Title = "Title",
                Description = "Description",
                StatusId = 1,
                PriorityId = 2,
                CreatedDate = DateTime.Now.AddDays(-5),
                ConclusionDate = DateTime.Now.AddDays(10),
                ConcludedDate = DateTime.Now,
                RemovedDate = DateTime.Now,
                Removed = true
            });
            tasks.Add(new Core.Entities.Task
            {
                TaskId = Guid.NewGuid(),
                Attendant = attendant,
                AuthorId = Guid.NewGuid(),
                Author = author,
                Title = "Title",
                Description = "Description",
                StatusId = 1,
                PriorityId = 2,
                CreatedDate = DateTime.Now.AddDays(-5),
                ConclusionDate = DateTime.Now.AddDays(10),
                RemovedDate = DateTime.Now,
                Removed = true
            });

            return tasks;
        }
    }
}
