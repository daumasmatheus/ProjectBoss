using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Moq;
using NUnit.Framework;
using ProjectBoss.Api.Configuration;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services;
using ProjectBoss.Api.Services.Interfaces;
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
    public class ProjectServiceTests
    {
        private readonly Mock<IPersonInProjectService> mockPersonInProjectService;

        private readonly Mock<IProjectRepository> mockProjectRepository;
        private readonly IConverter converter;
        private readonly IMapper mapper;

        Guid mockProjectId = Guid.Parse("E7D1AE95-C9A0-4ABB-932D-17DFFA7A1E0C");

        ProjectDto newProject;

        public ProjectServiceTests()
        {
            mockPersonInProjectService = new Mock<IPersonInProjectService>();

            mockProjectRepository = new Mock<IProjectRepository>();

            converter = new SynchronizedConverter(new PdfTools());

            mapper = new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper();

            newProject = new ProjectDto
            {
                ProjectId = mockProjectId,
                AuthorId = Guid.NewGuid(),
                Title = "new project",
                Description = "new project",
                StartDate = DateTime.Now,
                ConclusionDate = DateTime.Now,
                Tasks = new List<CreateProjectTaskDto>
                {
                    new CreateProjectTaskDto { ProjectId = mockProjectId, AttendantId = Guid.NewGuid(), AuthorId = Guid.NewGuid(), StatusId = 1, PriorityId = 1, Title = "Task", Description = "Task", ConclusionDate = DateTime.Now },
                    new CreateProjectTaskDto { ProjectId = mockProjectId, AttendantId = Guid.NewGuid(), AuthorId = Guid.NewGuid(), StatusId = 1, PriorityId = 1, Title = "Task", Description = "Task", ConclusionDate = DateTime.Now }
                },
                AttendantIds = new List<Guid> { Guid.NewGuid() }
            };
        }

        [TearDown]
        public async Task AfterEachTest()
        {
            mockProjectRepository.Invocations.Clear();
            mockPersonInProjectService.Invocations.Clear();
        }

        [Test]
        public async Task CreateNewProject_GivenValidProjectData_ShouldCreateNewProject()
        {
            mockProjectRepository.Setup(s => s.InsertAsNoTracking(It.IsAny<Core.Entities.Project>()))
                                 .ReturnsAsync(true);

            mockPersonInProjectService.Setup(s => s.AddProjectAttendant(It.IsAny<PersonInProjectParameterDto>()))
                                       .ReturnsAsync(true);

            var projectService = new ProjectService(mockProjectRepository.Object,
                                                    mockPersonInProjectService.Object,
                                                    converter,
                                                    mapper);

            var result = await projectService.CreateNewProject(newProject);

            Assert.IsNotNull(result);
            Assert.AreEqual(newProject, result);
        }

        [Test]
        public async Task CreateNewProject_GivenThatTheProjectWasntSaved_ShouldCreateNewProject()
        {
            mockProjectRepository.Setup(s => s.InsertAsNoTracking(It.IsAny<Core.Entities.Project>()))
                                 .ReturnsAsync(false);

            var projectService = new ProjectService(mockProjectRepository.Object,
                                                    mockPersonInProjectService.Object,
                                                    converter,
                                                    mapper);

            var result = await projectService.CreateNewProject(newProject);

            Assert.IsNull(result);            
        }

        [Test]
        public async Task CreateNewProject_GivenThatTheAttendantsWasntSaved_ShouldCreateNewProject()
        {
            mockProjectRepository.Setup(s => s.InsertAsNoTracking(It.IsAny<Core.Entities.Project>()))
                                 .ReturnsAsync(true);

            mockPersonInProjectService.Setup(s => s.AddProjectAttendant(It.IsAny<PersonInProjectParameterDto>()))
                                      .ReturnsAsync(false);

            var projectService = new ProjectService(mockProjectRepository.Object,
                                                    mockPersonInProjectService.Object,
                                                    converter,
                                                    mapper);

            var result = await projectService.CreateNewProject(newProject);

            Assert.IsNull(result);
        }

        [Test]
        public async Task EditProjectData_GivenValidDataToEditProject_ShouldEditProject()
        {
            mockProjectRepository.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Project, bool>>>()))
                                 .ReturnsAsync(GetProjects().First());

            mockProjectRepository.Setup(s => s.Update(It.IsAny<Core.Entities.Project>())).Returns(Task.CompletedTask);
            mockProjectRepository.Setup(s => s.SaveChanges()).ReturnsAsync(true);

            var projectService = new ProjectService(mockProjectRepository.Object, mockPersonInProjectService.Object, converter, mapper);

            var result = await projectService.EditProjectData(newProject);

            mockProjectRepository.Verify(v => v.Update(It.IsAny<Core.Entities.Project>()), Times.Once);
            mockProjectRepository.Verify(v => v.SaveChanges(), Times.Once);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task EditProjectData_GivenUnexistentProject_ShouldntEditProject()
        {
            mockProjectRepository.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Project, bool>>>()))
                                 .ReturnsAsync((Core.Entities.Project)null);            

            var projectService = new ProjectService(mockProjectRepository.Object, mockPersonInProjectService.Object, converter, mapper);

            var result = await projectService.EditProjectData(newProject);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetProjectDataById_GivenAnExistingProjectId_ShouldReturnProject()
        {
            mockProjectRepository.Setup(s => s.GetProjectDataById(It.IsAny<Guid>()))
                                 .ReturnsAsync(GetProjects().Where(x => x.ProjectId == mockProjectId).First());

            var projectService = new ProjectService(mockProjectRepository.Object, mockPersonInProjectService.Object, converter, mapper);

            var result = await projectService.GetProjectDataById(mockProjectId);

            Assert.NotNull(result);
            Assert.AreEqual(mockProjectId, result.ProjectId);
        }

        [Test]
        public async Task GetProjectDataById_GivenAnUnexistingProjectId_ShouldReturnNull()
        {
            mockProjectRepository.Setup(s => s.GetProjectDataById(It.IsAny<Guid>()))
                                 .ReturnsAsync((Core.Entities.Project)null);

            var projectService = new ProjectService(mockProjectRepository.Object, mockPersonInProjectService.Object, converter, mapper);

            var result = await projectService.GetProjectDataById(mockProjectId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task ExportProjectAsPdf_GivenThatThereIsProjectsToCreateTheDocument_ShouldReturnsTheByteArray()
        {
            mockProjectRepository.Setup(s => s.GetProjectById(It.IsAny<Guid>()))
                                 .ReturnsAsync(GetProjects().First());

            var projectService = new ProjectService(mockProjectRepository.Object, mockPersonInProjectService.Object, converter, mapper);

            var result = await projectService.ExportProjectAsPdf(mockProjectId);

            Assert.NotNull(result);
            CollectionAssert.IsNotEmpty(result);
        }

        [Test]
        public async Task ExportProjectAsPdf_GivenThatTheProjectWasntFound_ShouldReturnsNull()
        {
            mockProjectRepository.Setup(s => s.GetProjectById(It.IsAny<Guid>()))
                                 .ReturnsAsync((Core.Entities.Project)null);

            var projectService = new ProjectService(mockProjectRepository.Object, mockPersonInProjectService.Object, converter, mapper);

            var result = await projectService.ExportProjectAsPdf(mockProjectId);

            Assert.Null(result);            
        }

        [Test]
        public async Task ExportProjectAsXlsl_GivenThatThereIsProjectsToCreateTheDocument_ShouldReturnsTheByteArray()
        {
            mockProjectRepository.Setup(s => s.GetProjectById(It.IsAny<Guid>()))
                                 .ReturnsAsync(GetProjects().First());

            var projectService = new ProjectService(mockProjectRepository.Object, mockPersonInProjectService.Object, converter, mapper);

            var result = await projectService.ExportProjectAsXlsl(mockProjectId);

            Assert.NotNull(result);
            CollectionAssert.IsNotEmpty(result);
        }

        [Test]
        public async Task ExportProjectAsXlsl_GivenThatTheProjectWasntFount_ShouldReturnNull()
        {
            mockProjectRepository.Setup(s => s.GetProjectById(It.IsAny<Guid>()))
                                 .ReturnsAsync((Core.Entities.Project)null);

            var projectService = new ProjectService(mockProjectRepository.Object, mockPersonInProjectService.Object, converter, mapper);

            var result = await projectService.ExportProjectAsXlsl(mockProjectId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetProjectDataForDropdown_GivenThatThereIsProjects_ShouldReturnData()
        {
            mockProjectRepository.Setup(s => s.GetAll()).ReturnsAsync(GetProjects());

            var projectService = new ProjectService(mockProjectRepository.Object, mockPersonInProjectService.Object, converter, mapper);

            var result = await projectService.GetProjectDataForDropdown();

            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ProjectDataForDropdownDto));
        }

        [Test]
        public async Task GetProjectDataForDropdown_GivenThatThereIsntProjects_ShouldReturnNull()
        {
            mockProjectRepository.Setup(s => s.GetAll()).ReturnsAsync((IEnumerable<Core.Entities.Project>)null);

            var projectService = new ProjectService(mockProjectRepository.Object, mockPersonInProjectService.Object, converter, mapper);

            var result = await projectService.GetProjectDataForDropdown();

            Assert.IsNull(result);
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
    }
}
