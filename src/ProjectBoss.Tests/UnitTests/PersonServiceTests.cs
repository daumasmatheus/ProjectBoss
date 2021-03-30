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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.UnitTests
{
    [TestFixture]
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> mockPersonRepository;
        private readonly IMapper mapper;

        Guid mockPersonId = Guid.Parse("C27DF3CE-101A-4071-A137-7BC95B3074BF");
        Guid mockUserId = Guid.Parse("139D10CE-1643-44E2-818F-6C722D12495C");

        public PersonServiceTests()
        {
            mockPersonRepository = new Mock<IPersonRepository>();
            mapper = new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper();
        }

        [TearDown]
        public async Task AfterEachTest()
        {
            mockPersonRepository.Invocations.Clear();
        }

        [Test]
        public async Task CreatePerson_GivenAValidPersonData_ShouldCreateNewPerson()
        {
            var person = new CreatePersonDto
            {
                UserId = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Test",
                Role = "test"
            };

            mockPersonRepository.Setup(x => x.Create(It.IsAny<Core.Entities.Person>()))
                                .Returns(Task.CompletedTask);
            mockPersonRepository.Setup(x => x.SaveChanges())
                                .ReturnsAsync(true);

            var personService = new PersonService(mockPersonRepository.Object, mapper);

            var result = await personService.CreatePerson(person);

            mockPersonRepository.Verify(x => x.SaveChanges(), Times.Once);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task CreatePerson_GivenAnInvalidPersonData_ShouldntCreateNewPersonAndReturnFalse()
        {
            var person = new CreatePersonDto
            {
                FirstName = "Test",
                LastName = "Test",
                Role = "test"
            };

            mockPersonRepository.Setup(x => x.Create(It.IsAny<Core.Entities.Person>()))
                                .Returns(Task.CompletedTask);
            mockPersonRepository.Setup(x => x.SaveChanges())
                                .ReturnsAsync(false);

            var personService = new PersonService(mockPersonRepository.Object, mapper);

            var result = await personService.CreatePerson(person);

            mockPersonRepository.Verify(x => x.SaveChanges(), Times.Once);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetPersonalDataByUserId_GivenAValidAndExistentUserId_ShouldReturnPersonData()
        {
            mockPersonRepository.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Person, bool>>>()))
                                .Returns(Task.FromResult(GetPeople().Where(x => x.UserId == mockUserId.ToString()).FirstOrDefault()));

            var personService = new PersonService(mockPersonRepository.Object, mapper);

            var result = await personService.GetPersonalDataByUserId(mockUserId.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(mockUserId.ToString(), result.UserId);
        }

        [Test]
        public async Task GetPersonalDataByUserId_GivenAValidAndUnexistentUserId_ShouldReturnNull()
        {
            mockPersonRepository.Setup(x => x.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Person, bool>>>()))
                                .Returns(Task.FromResult<Core.Entities.Person>(null));

            var personService = new PersonService(mockPersonRepository.Object, mapper);

            var result = await personService.GetPersonalDataByUserId(mockUserId.ToString());

            Assert.Null(result);
        }

        [Test]
        public async Task EditPerson_GivenValidDataForExistingPerson_ShouldEditPerson()
        {
            var editPerson = new EditPersonDataDto
            {
                PersonId = mockPersonId,
                UserId = mockUserId,
                FirstName = "test",
                LastName = "test",
                Role = "role test",
                Company = "company test",
                Country = "country test"
            };

            mockPersonRepository.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Person, bool>>>()))
                                .Returns(Task.FromResult(GetPeople().First()));
            mockPersonRepository.Setup(s => s.Update(It.IsAny<Core.Entities.Person>())).Returns(Task.CompletedTask);
            mockPersonRepository.Setup(s => s.SaveChanges()).ReturnsAsync(true);

            var personService = new PersonService(mockPersonRepository.Object, mapper);

            var result = await personService.EditPerson(editPerson);

            mockPersonRepository.Verify(v => v.Update(It.IsAny<Core.Entities.Person>()), Times.Once);
            mockPersonRepository.Verify(v => v.SaveChanges(), Times.Once);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task EditPerson_GivenValidDataForUnexistingPerson_ShouldntEditPersonAndReturnFalse()
        {
            var editPerson = new EditPersonDataDto
            {
                PersonId = mockPersonId,
                UserId = mockUserId,
                FirstName = "test",
                LastName = "test",
                Role = "role test",
                Company = "company test",
                Country = "country test"
            };

            mockPersonRepository.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Person, bool>>>()))
                                .Returns(Task.FromResult<Core.Entities.Person>(null));            

            var personService = new PersonService(mockPersonRepository.Object, mapper);

            var result = await personService.EditPerson(editPerson);            

            Assert.IsFalse(result);
        }         

        [Test]
        public async Task GetPersonalDataByPersonId_GivenThatThePersonExists_ShouldReturnItsData()
        {
            mockPersonRepository.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Person, bool>>>()))
                                .Returns(Task.FromResult(GetPeople().Where(x => x.PersonId == mockPersonId).FirstOrDefault()));

            var personService = new PersonService(mockPersonRepository.Object, mapper);

            var result = await personService.GetPersonalDataByPersonId(mockPersonId);

            Assert.NotNull(result);
            Assert.AreEqual(mockPersonId, result.PersonId);
        }

        [Test]
        public async Task GetPersonalDataByPersonId_GivenThatThePersonDontExists_ShouldReturnNull()
        {
            mockPersonRepository.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<Core.Entities.Person, bool>>>()))
                                .Returns(Task.FromResult<Core.Entities.Person>(null));

            var personService = new PersonService(mockPersonRepository.Object, mapper);

            var result = await personService.GetPersonalDataByPersonId(mockPersonId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllPerson_ThereIsDataToReturn_ShouldReturnListOfPerson()
        {
            mockPersonRepository.Setup(s => s.GetAllPersonWithUser()).Returns(Task.FromResult(GetPeople()));

            var personService = new PersonService(mockPersonRepository.Object, mapper);

            var result = await personService.GetAllPerson();

            Assert.IsTrue(result.Any());
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(PersonFullDto));
        }

        [Test]
        public async Task GetAllPerson_ThereIsNoDataToReturn_ShouldReturnNull()
        {
            mockPersonRepository.Setup(s => s.GetAllPersonWithUser())
                                .Returns(Task.FromResult<List<Core.Entities.Person>>(null));

            var personService = new PersonService(mockPersonRepository.Object, mapper);

            var result = await personService.GetAllPerson();

            Assert.IsTrue(!result.Any());
            CollectionAssert.IsEmpty(result);
        }

        public List<Core.Entities.Person> GetPeople()
        {
            List<Core.Entities.Person> people = new List<Core.Entities.Person>();

            people.Add(new Core.Entities.Person
            {
                PersonId = mockPersonId,
                UserId = mockUserId.ToString(),
                FirstName = "Test",
                LastName = "test",
                CreatedDate = DateTime.Now
            });
            people.Add(new Core.Entities.Person
            {
                PersonId = mockPersonId,
                UserId = mockUserId.ToString(),
                FirstName = "Test",
                LastName = "test",
                CreatedDate = DateTime.Now
            });
            people.Add(new Core.Entities.Person
            {
                PersonId = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Test",
                LastName = "test",
                CreatedDate = DateTime.Now
            });
            people.Add(new Core.Entities.Person
            {
                PersonId = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Test",
                LastName = "test",
                CreatedDate = DateTime.Now
            });

            return people;
        }
    }
}
