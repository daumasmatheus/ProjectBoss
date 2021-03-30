using AutoMapper;
using Moq;
using NUnit.Framework;
using ProjectBoss.Api.Configuration;
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
    public class StatisticsServiceTests
    {
        private readonly Mock<IProjectRepository> mockProjectRepository;
        private readonly Mock<IUserRepository> mockUserRepository;
        private readonly Mock<ITaskRepository> mockTaskRepository;
        private readonly Mock<IPersonInProjectRepository> mockPersonInProjectRepository;
        private readonly IMapper mapper;

        Guid mockAuthorId = Guid.Parse("C7ED37F2-D609-4CBB-B85B-277C9A255A37");
        Guid mockAttendantId = Guid.Parse("6115B132-51CE-42E6-AA84-E35B9D2142C7");
        Guid mockProjectId = Guid.Parse("CE9E3632-6AB4-42E6-A543-9B676559C712");
        Guid mockTaskId = Guid.Parse("62C0CAB6-D616-4C64-A483-4DC25B3995EC");
        Guid mockPersonId = Guid.Parse("F1C7C4ED-31C0-49F0-B9B9-132916B9B65E");

        Core.Entities.Project mockProject;
        Core.Entities.Person mockPerson;

        public StatisticsServiceTests()
        {
            mockProjectRepository = new Mock<IProjectRepository>();
            mockUserRepository = new Mock<IUserRepository>();
            mockTaskRepository = new Mock<ITaskRepository>();
            mockPersonInProjectRepository = new Mock<IPersonInProjectRepository>();

            mapper = new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper();

            mockPerson = new Core.Entities.Person
            {
                PersonId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                FirstName = "NewPerson",
                LastName = "NewPerson",
                IsActive = true
            };

            mockProject = new Core.Entities.Project
            {
                ProjectId = mockProjectId,
                AuthorId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                StartDate = DateTime.Now,
                ConclusionDate = DateTime.Now.AddDays(15),
                Description = "NewProject",
                Title = "NewProject"
            };
        }

        [Test]
        public async Task GetPersonOverviewStatistics_GivenAnExistingPersonId_ShouldReturnPersonOverviewStatisticsData()
        {
            mockTaskRepository.Setup(s => s.GetManyByCondition(It.IsAny<Expression<Func<Core.Entities.Task, bool>>>()))
                              .ReturnsAsync(GetTasks());
            mockPersonInProjectRepository.Setup(s => s.GetOpenPersonProjectsWithChildEntities(It.IsAny<Guid>()))
                                         .ReturnsAsync(GetPersonInProjects());

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetPersonOverviewStatistics(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ConcludedTasks);
            Assert.IsNotNull(result.TasksDueSoon);
            Assert.IsNotNull(result.RecentProjects);
        }

        [Test]
        public async Task GetPersonOverviewStatistics_GivenAnPersonIdAndThereIsNoData_ShouldReturnEmptyResult()
        {
            mockTaskRepository.Setup(s => s.GetManyByCondition(It.IsAny<Expression<Func<Core.Entities.Task, bool>>>()))
                              .ReturnsAsync((IEnumerable<Core.Entities.Task>)null);
            mockPersonInProjectRepository.Setup(s => s.GetOpenPersonProjectsWithChildEntities(It.IsAny<Guid>()))
                                         .ReturnsAsync((IEnumerable<Domain.Entities.PersonInProject>)null);

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetPersonOverviewStatistics(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsEmpty(result.ConcludedTasks);
            Assert.IsEmpty(result.TasksDueSoon);
            Assert.IsEmpty(result.RecentProjects);
        }

        [Test]
        public async Task GetNewAndClosedTasksByDateByProject_GivenAnExistingProjectId_ShouldReturnData()
        {
            mockProjectRepository.Setup(s => s.GetProjectById(It.IsAny<Guid>()))
                                 .ReturnsAsync(GetProjects().First());

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetNewAndClosedTasksByDateByProject(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task GetNewAndClosedTasksByDateByProject_GivenAProjectIdAndThereIsNoData_ShouldReturnEmptyResult()
        {
            mockProjectRepository.Setup(s => s.GetProjectById(It.IsAny<Guid>()))
                                 .ReturnsAsync((Core.Entities.Project)null);

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetNewAndClosedTasksByDateByProject(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetOpenAndOnGoingTasksByPersonInProject_GivenAnExistingProjectId_ShouldReturnData()
        {
            mockProjectRepository.Setup(s => s.GetProjectById(It.IsAny<Guid>()))
                                 .ReturnsAsync(GetProjects().First());

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetOpenAndOnGoingTasksByPersonInProject(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task GetOpenAndOnGoingTasksByPersonInProject_GivenAnExistingProjectIdAndThereIsNoData_ShouldReturnEmptyResult()
        {
            mockProjectRepository.Setup(s => s.GetProjectById(It.IsAny<Guid>()))
                                 .ReturnsAsync((Core.Entities.Project)null);

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetOpenAndOnGoingTasksByPersonInProject(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetTasksStatusByProject_GivenAnExistingProjectId_ShouldReturnData()
        {
            mockProjectRepository.Setup(s => s.GetProjectById(It.IsAny<Guid>()))
                                 .ReturnsAsync(GetProjects().First());

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetTasksStatusByProject(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task GetTasksStatusByProject_GivenAnExistingProjectIdAndThereIsNoData_ShouldReturnEmptyResult()
        {
            mockProjectRepository.Setup(s => s.GetProjectById(It.IsAny<Guid>()))
                                 .ReturnsAsync((Core.Entities.Project)null);

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetTasksStatusByProject(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetCreatedUsers_ShouldReturnData()
        {
            mockUserRepository.Setup(s => s.GetAll())
                              .ReturnsAsync(GetUsers());

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetCreatedUsers();

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Name));
            Assert.IsNotEmpty(result.Series);
        }

        [Test]
        public async Task GetTotalCreatedTasksByDate_GivenThatThereIsTasks_ShouldReturnData()
        {
            mockTaskRepository.Setup(s => s.GetAll()).ReturnsAsync(GetTasks());

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetTotalCreatedTasksByDate();

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Name));
            Assert.IsNotEmpty(result.Series);
        }

        [Test]
        public async Task GetTotalCreatedTasksByDate_GivenThatThereIsntTasks_ShouldReturnNull()
        {
            mockTaskRepository.Setup(s => s.GetAll()).ReturnsAsync((IEnumerable<Core.Entities.Task>)null);

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetTotalCreatedTasksByDate();

            Assert.IsNull(result);            
        }

        [Test]
        public async Task GetTotalConcludedTasksByDate_GivenThatThereIsTasks_ShouldReturnData()
        {
            mockTaskRepository.Setup(s => s.GetAll()).ReturnsAsync(GetTasks());

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetTotalConcludedTasksByDate();

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Name));
            Assert.IsNotEmpty(result.Series);
        }

        [Test]
        public async Task GetTotalConcludedTasksByDate_GivenThatThereIsntTasks_ShouldReturnNull()
        {
            mockTaskRepository.Setup(s => s.GetAll()).ReturnsAsync((IEnumerable<Core.Entities.Task>)null);

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetTotalConcludedTasksByDate();

            Assert.IsNull(result);            
        }

        [Test]
        public async Task GetTotalCreatedProjectsByDate_GivenThatThereIsProjects_ShouldReturnData()
        {
            mockProjectRepository.Setup(s => s.GetAll()).ReturnsAsync(GetProjects());

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetTotalCreatedProjectsByDate();

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Name));
            Assert.IsNotEmpty(result.Series);
        }

        [Test]
        public async Task GetTotalCreatedProjectsByDate_GivenThatThereIsntProjects_ShouldReturnNull()
        {
            mockProjectRepository.Setup(s => s.GetAll()).ReturnsAsync((IEnumerable<Core.Entities.Project>)null);

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetTotalCreatedProjectsByDate();

            Assert.IsTrue(string.IsNullOrEmpty(result.Name));
            Assert.IsNull(result.Series);
        }

        [Test]
        public async Task GetTotalConcludedProjectsByDate_GivenThatThereIsProjects_ShouldReturnData()
        {
            mockProjectRepository.Setup(s => s.GetAll()).ReturnsAsync(GetProjects());

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetTotalConcludedProjectsByDate();

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Name));
            Assert.IsNotNull(result.Series);
        }

        [Test]
        public async Task GetTotalConcludedProjectsByDate_GivenThatThereIsntProjects_ShouldReturnNull()
        {
            mockProjectRepository.Setup(s => s.GetAll()).ReturnsAsync((IEnumerable<Core.Entities.Project>)null);

            var statisticsService = new StatisticsService(mockProjectRepository.Object,
                                                          mockUserRepository.Object,
                                                          mockTaskRepository.Object,
                                                          mockPersonInProjectRepository.Object,
                                                          mapper);

            var result = await statisticsService.GetTotalConcludedProjectsByDate();

            Assert.IsTrue(string.IsNullOrEmpty(result.Name));
            Assert.IsNull(result.Series);
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

        private List<Domain.Entities.PersonInProject> GetPersonInProjects()
        {
            List<Domain.Entities.PersonInProject> personInProjects = new List<Domain.Entities.PersonInProject>();

            personInProjects.Add(new Domain.Entities.PersonInProject
            {
                Id = 1,
                PersonId = mockPersonId,
                Person = mockPerson,
                ProjectId = mockProjectId
            });
            personInProjects.Add(new Domain.Entities.PersonInProject
            {
                Id = 2,
                PersonId = mockPersonId,
                Person = mockPerson,
                ProjectId = mockProjectId
            });
            personInProjects.Add(new Domain.Entities.PersonInProject
            {
                Id = 3,
                PersonId = mockPersonId,
                Person = mockPerson,
                ProjectId = mockProjectId
            });
            personInProjects.Add(new Domain.Entities.PersonInProject
            {
                Id = 4,
                PersonId = Guid.NewGuid(),
                Person = mockPerson,
                ProjectId = Guid.NewGuid()
            });
            personInProjects.Add(new Domain.Entities.PersonInProject
            {
                Id = 5,
                PersonId = Guid.NewGuid(),
                Person = mockPerson,
                ProjectId = Guid.NewGuid()
            });

            return personInProjects;
        }

        public List<Core.Entities.Project> GetProjects()
        {
            List<Core.Entities.Project> projects = new List<Core.Entities.Project>();
            projects.Add(new Core.Entities.Project
            {
                ProjectId = mockProjectId,
                AuthorId = Guid.NewGuid(),
                Author = new Core.Entities.Person
                {
                    PersonId = Guid.NewGuid(),
                    FirstName = "Test",
                    LastName = "Test",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                CreatedDate = DateTime.Now,
                StartDate = DateTime.Now,
                ConclusionDate = DateTime.Now.AddDays(15),
                Description = "NewProject",
                Removed = false,
                Title = "NewProject",
                Tasks = new List<Core.Entities.Task>
                {
                    new Core.Entities.Task
                    {
                        ProjectId = mockProjectId,
                        AttendantId = Guid.NewGuid(),
                        Attendant = new Core.Entities.Person
                        {
                            PersonId = Guid.NewGuid(),
                            UserId = Guid.NewGuid().ToString(),
                            FirstName = "Test",
                            LastName = "test",
                            CreatedDate = DateTime.Now
                        },
                        AuthorId = Guid.NewGuid(),
                        Author = new Core.Entities.Person
                        {
                            PersonId = Guid.NewGuid(),
                            UserId = Guid.NewGuid().ToString(),
                            FirstName = "Test",
                            LastName = "test",
                            CreatedDate = DateTime.Now
                        },
                        StatusId = 1,
                        Status = new Core.Entities.Status { Name = "asd", StatusId = 1 },
                        PriorityId = 1,
                        Priority = new Core.Entities.Priority { Name = "asd", PriorityId = 1 },
                        Title = "Task",
                        Description = "Task",
                        ConclusionDate = DateTime.Now
                    },
                },
                PersonInProject = new List<Domain.Entities.PersonInProject>
                {
                    new Domain.Entities.PersonInProject
                    {
                        Id = 1,
                        PersonId = Guid.NewGuid(),
                        Person = new Core.Entities.Person
                        {
                            PersonId = Guid.NewGuid(),
                            UserId = Guid.NewGuid().ToString(),
                            FirstName = "Test",
                            LastName = "test",
                            CreatedDate = DateTime.Now
                        },
                        ProjectId = mockProjectId }
                }
            });

            projects.Add(new Core.Entities.Project
            {
                ProjectId = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Author = new Core.Entities.Person
                {
                    PersonId = Guid.NewGuid(),
                    FirstName = "Test",
                    LastName = "Test",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                CreatedDate = DateTime.Now,
                StartDate = DateTime.Now,
                ConclusionDate = DateTime.Now.AddDays(15),
                Description = "NewProject",
                Removed = false,
                Title = "NewProject",
                Tasks = new List<Core.Entities.Task>
                {
                    new Core.Entities.Task { ProjectId = mockProjectId, AttendantId = Guid.NewGuid(), AuthorId = Guid.NewGuid(), StatusId = 1, PriorityId = 1, Title = "Task", Description = "Task", ConclusionDate = DateTime.Now },
                    new Core.Entities.Task { ProjectId = mockProjectId, AttendantId = Guid.NewGuid(), AuthorId = Guid.NewGuid(), StatusId = 1, PriorityId = 1, Title = "Task", Description = "Task", ConclusionDate = DateTime.Now }
                }
            });

            return projects;
        }

        public List<Domain.Extensions.ApplicationUser> GetUsers()
        {
            List<Domain.Extensions.ApplicationUser> users = new List<Domain.Extensions.ApplicationUser>();

            users.Add(new Domain.Extensions.ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@test.com",
                NormalizedEmail = "test@test.com",
                UserName = "test@test.com",
                NormalizedUserName = "test@test.com",
                CreatedDate = DateTime.Now,
                Provider = "LOCAL"
            });

            users.Add(new Domain.Extensions.ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test2@test.com",
                NormalizedEmail = "test2@test.com",
                UserName = "test2@test.com",
                NormalizedUserName = "test2@test.com",
                CreatedDate = DateTime.Now,
                Provider = "GOOGLE"
            });

            return users;
        }
    }
}
