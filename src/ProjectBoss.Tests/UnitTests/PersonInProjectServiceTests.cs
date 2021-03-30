using AutoMapper;
using Moq;
using NUnit.Framework;
using ProjectBoss.Api.Configuration;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services;
using ProjectBoss.Core.Entities;
using ProjectBoss.Data.Repositories.Interfaces;
using ProjectBoss.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.UnitTests
{
    [TestFixture]
    public class PersonInProjectServiceTests
    {
        private readonly Mock<IPersonInProjectRepository> mockPersonInProjectRepository;
        private readonly Mock<IPersonRepository> mockPersonRepository;
        private readonly Mock<IProjectRepository> mockProjectRepository;
        private readonly IMapper mapper;

        Project mockProject;
        Person mockPerson;

        Guid mockPersonId = Guid.Parse("F1C7C4ED-31C0-49F0-B9B9-132916B9B65E");
        Guid mockProjedtId = Guid.Parse("80F77362-627C-43CB-9194-69B6EA801EAF");

        public PersonInProjectServiceTests()
        {
            mockPersonInProjectRepository = new Mock<IPersonInProjectRepository>();
            mockPersonRepository = new Mock<IPersonRepository>();
            mockProjectRepository = new Mock<IProjectRepository>();

            mapper = new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper();

            mockPerson = new Person
            {
                PersonId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                FirstName = "NewPerson",
                LastName = "NewPerson",
                IsActive = true
            };

            mockProject = new Project
            {
                ProjectId = mockProjedtId,
                AuthorId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                StartDate = DateTime.Now,
                ConclusionDate = DateTime.Now.AddDays(15),
                Description = "NewProject",
                Title = "NewProject"
            };
        }

        [TearDown]
        public async System.Threading.Tasks.Task AfterEachTest()
        {
            mockPersonInProjectRepository.Invocations.Clear();
            mockPersonRepository.Invocations.Clear();
            mockProjectRepository.Invocations.Clear();
        }

        [Test]
        public async System.Threading.Tasks.Task AddProjectAttendant_GivenExistingProjectIdAndAttendantIds_ShouldAddAttendantToProject()
        {
            var parameters = new PersonInProjectParameterDto
            {
                ProjectId = Guid.NewGuid(),
                AttendantIds = new Guid[] { Guid.NewGuid() }
            };

            mockProjectRepository.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<Project, bool>>>()))
                                 .ReturnsAsync(mockProject);
            mockPersonRepository.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<Person, bool>>>()))
                                .ReturnsAsync(mockPerson);
            mockPersonInProjectRepository.Setup(s => s.Create(It.IsAny<Domain.Entities.PersonInProject>()))
                                         .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockPersonInProjectRepository.Setup(s => s.SaveChanges())
                                         .ReturnsAsync(true);

            var personInProjectService = new PersonInProjectService(mapper, 
                                                                    mockPersonInProjectRepository.Object, 
                                                                    mockPersonRepository.Object, 
                                                                    mockProjectRepository.Object);

            var result = await personInProjectService.AddProjectAttendant(parameters);

            mockProjectRepository.Verify(v => v.GetSingleByCondition(It.IsAny<Expression<Func<Project, bool>>>()), Times.Once);
            mockPersonRepository.Verify(v => v.GetSingleByCondition(It.IsAny<Expression<Func<Person, bool>>>()), Times.AtLeastOnce);
            mockPersonInProjectRepository.Verify(v => v.SaveChanges(), Times.Once);

            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task AddProjectAttendant_GivenExistingProjectIdAndAnUnexistingAttendantId_ShouldntAddNewAttendantAndReturnFalse()
        {
            Person expectedPersonReturn = null;

            var parameters = new PersonInProjectParameterDto
            {
                ProjectId = Guid.NewGuid(),
                AttendantIds = new Guid[] { Guid.NewGuid() }
            };

            mockProjectRepository.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<Project, bool>>>()))
                                 .ReturnsAsync(mockProject);
            mockPersonRepository.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<Person, bool>>>()))
                                .Returns(System.Threading.Tasks.Task.FromResult<Person>(null));
            mockPersonInProjectRepository.Setup(s => s.Create(It.IsAny<Domain.Entities.PersonInProject>()))
                                         .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockPersonInProjectRepository.Setup(s => s.SaveChanges())
                                         .ReturnsAsync(true);

            var personInProjectService = new PersonInProjectService(mapper,
                                                                    mockPersonInProjectRepository.Object,
                                                                    mockPersonRepository.Object,
                                                                    mockProjectRepository.Object);

            var result = await personInProjectService.AddProjectAttendant(parameters);

            mockProjectRepository.Verify(v => v.GetSingleByCondition(It.IsAny<Expression<Func<Project, bool>>>()), Times.Once);
            mockPersonRepository.Verify(v => v.GetSingleByCondition(It.IsAny<Expression<Func<Person, bool>>>()), Times.AtLeastOnce);

            Assert.IsFalse(result);
        }

        [Test]
        public async System.Threading.Tasks.Task AddProjectAttendant_GivenAnUnexistingProjectId_ShouldntAddNewAttendantAndReturnFalse()
        {
            var parameters = new PersonInProjectParameterDto
            {
                ProjectId = Guid.NewGuid(),
                AttendantIds = new Guid[] { Guid.NewGuid() }
            };

            mockProjectRepository.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<Project, bool>>>()))
                                 .Returns(System.Threading.Tasks.Task.FromResult<Project>(null));           
            

            var personInProjectService = new PersonInProjectService(mapper,
                                                                    mockPersonInProjectRepository.Object,
                                                                    mockPersonRepository.Object,
                                                                    mockProjectRepository.Object);

            var result = await personInProjectService.AddProjectAttendant(parameters);

            mockProjectRepository.Verify(v => v.GetSingleByCondition(It.IsAny<Expression<Func<Project, bool>>>()), Times.Once);

            Assert.IsFalse(result);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectsWherePersonIsAttendant_ThereIsProjectsForTheGivenAttendant_ShouldReturnProjects()
        {
            mockPersonInProjectRepository.Setup(x => x.GetProjectsByPersonWithProjectData(It.IsAny<Guid>()))
                                         .Returns(System.Threading.Tasks.Task.FromResult(GetPersonInProjects().Where(x => x.PersonId == mockPersonId)));

            var personInProjectService = new PersonInProjectService(mapper,
                                                                    mockPersonInProjectRepository.Object,
                                                                    mockPersonRepository.Object,
                                                                    mockProjectRepository.Object);

            var result = await personInProjectService.GetProjectsWherePersonIsAttendant(mockPersonId);

            Assert.IsTrue(result.Any());
            Assert.AreEqual(GetPersonInProjects().Count(x => x.PersonId == mockPersonId), result.Count());
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectsWherePersonIsAttendant_ThereIsNoProjectsForTheGivenAttendant_ShouldReturnNull()
        {
            mockPersonInProjectRepository.Setup(x => x.GetProjectsByPersonWithProjectData(It.IsAny<Guid>()))
                                         .Returns(System.Threading.Tasks.Task.FromResult<IEnumerable<PersonInProject>>(null));

            var personInProjectService = new PersonInProjectService(mapper,
                                                                    mockPersonInProjectRepository.Object,
                                                                    mockPersonRepository.Object,
                                                                    mockProjectRepository.Object);

            var result = await personInProjectService.GetProjectsWherePersonIsAttendant(It.IsAny<Guid>());

            Assert.IsTrue(!result.Any());
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectAttendants_ThereIsAttendantsInTheProject_ShouldReturnTheProjectAttendants()
        {
            mockPersonInProjectRepository.Setup(x => x.GetProjectAttendants(It.IsAny<Guid>()))
                                         .Returns(
                                            System.Threading.Tasks.Task.FromResult(GetPersonInProjects().Where(x => x.ProjectId == mockProjedtId))
                                            );

            var personInProjectService = new PersonInProjectService(mapper,
                                                                    mockPersonInProjectRepository.Object,
                                                                    mockPersonRepository.Object,
                                                                    mockProjectRepository.Object);

            var result = await personInProjectService.GetProjectAttendants(mockProjedtId);

            Assert.IsTrue(result.Any());
            Assert.AreEqual(GetPersonInProjects().Count(x => x.ProjectId == mockProjedtId), result.Count());
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectAttendants_ThereIsntAttendantsInTheProject_ShouldReturnEmptyList()
        {
            mockPersonInProjectRepository.Setup(x => x.GetProjectAttendants(It.IsAny<Guid>()))
                                         .Returns(
                                            System.Threading.Tasks.Task.FromResult<IEnumerable<PersonInProject>>(null)
                                            );

            var personInProjectService = new PersonInProjectService(mapper,
                                                                    mockPersonInProjectRepository.Object,
                                                                    mockPersonRepository.Object,
                                                                    mockProjectRepository.Object);

            var result = await personInProjectService.GetProjectAttendants(mockProjedtId);

            Assert.IsTrue(!result.Any());
        }

        [Test]
        public async System.Threading.Tasks.Task RemoveProjectAttendant_TheAttendantExists_ShouldRemoveTheAttendant()
        {
            var parameters = new PersonInProjectParameterDto
            {
                ProjectId = Guid.NewGuid(),
                AttendantIds = new Guid[] { Guid.NewGuid() }
            };

            mockPersonInProjectRepository.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<PersonInProject, bool>>>()))
                                         .Returns(System.Threading.Tasks.Task.FromResult(GetPersonInProjects().First()));
            mockPersonInProjectRepository.Setup(x => x.Delete(It.IsAny<PersonInProject>()))
                                         .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockPersonInProjectRepository.Setup(x => x.SaveChanges())
                                         .Returns(System.Threading.Tasks.Task.FromResult(true));

            var personInProjectService = new PersonInProjectService(mapper,
                                                                    mockPersonInProjectRepository.Object,
                                                                    mockPersonRepository.Object,
                                                                    mockProjectRepository.Object);

            var result = await personInProjectService.RemoveProjectAttendant(parameters);

            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task RemoveProjectAttendant_TheAttendantDoesNotExists_ShouldReturnFalse()
        {
            var parameters = new PersonInProjectParameterDto
            {
                ProjectId = Guid.NewGuid(),
                AttendantIds = new Guid[] { Guid.NewGuid() }
            };

            mockPersonInProjectRepository.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<PersonInProject, bool>>>()))
                                         .Returns(System.Threading.Tasks.Task.FromResult<PersonInProject>(null));
            mockPersonInProjectRepository.Setup(x => x.SaveChanges())
                                         .ReturnsAsync(false);

            var personInProjectService = new PersonInProjectService(mapper,
                                                                    mockPersonInProjectRepository.Object,
                                                                    mockPersonRepository.Object,
                                                                    mockProjectRepository.Object);

            var result = await personInProjectService.RemoveProjectAttendant(parameters);

            Assert.IsFalse(result);
        }

        private List<PersonInProject> GetPersonInProjects()
        {
            List<PersonInProject> personInProjects = new List<PersonInProject>();

            personInProjects.Add(new PersonInProject
            {
                Id = 1,
                PersonId = mockPersonId,
                Person = mockPerson,
                ProjectId = mockProjedtId
            });
            personInProjects.Add(new PersonInProject
            {
                Id = 2,
                PersonId = mockPersonId,
                Person = mockPerson,
                ProjectId = mockProjedtId
            });
            personInProjects.Add(new PersonInProject
            {
                Id = 3,
                PersonId = mockPersonId,
                Person = mockPerson,
                ProjectId = mockProjedtId
            });
            personInProjects.Add(new PersonInProject
            {
                Id = 4,
                PersonId = Guid.NewGuid(),
                Person = mockPerson,
                ProjectId = Guid.NewGuid()
            });
            personInProjects.Add(new PersonInProject
            {
                Id = 5,
                PersonId = Guid.NewGuid(),
                Person = mockPerson,
                ProjectId = Guid.NewGuid()
            });

            return personInProjects;
        }        
    }
}
