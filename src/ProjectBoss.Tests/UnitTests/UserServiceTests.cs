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
using ProjectBoss.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.UnitTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> mockUserRepo;
        private readonly Mock<IPersonRepository> mockPersonRepo;
        private readonly Mock<IPersonService> mockPersonService;
        private readonly Mock<IAuthenticationService> mockAuthService;

        private readonly IMapper mapper;
        private readonly IConverter converter;

        Core.Entities.Person person;

        public UserServiceTests()
        {
            mockUserRepo = new Mock<IUserRepository>();
            mockPersonRepo = new Mock<IPersonRepository>();
            mockPersonService = new Mock<IPersonService>();
            mockAuthService = new Mock<IAuthenticationService>();

            mapper = new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper();
            converter = new SynchronizedConverter(new PdfTools());

            person = new Core.Entities.Person
            {
                PersonId = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Test",
                LastName = "test",
                CreatedDate = DateTime.Now
            };
        }

        [TearDown]
        public async Task AfterEachTest()
        {
            mockUserRepo.Invocations.Clear();
            mockPersonRepo.Invocations.Clear();
            mockPersonService.Invocations.Clear();
            mockAuthService.Invocations.Clear();
        }

        [Test]
        public async Task EditUser_GivenValidUserData_ShouldEditUserData()
        {
            mockAuthService.Setup(s => s.EditRole(It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync(true);

            mockUserRepo.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                        .ReturnsAsync(GetUsers().First());
            mockUserRepo.Setup(s => s.Update(It.IsAny<ApplicationUser>()))
                        .Returns(Task.CompletedTask);
            mockUserRepo.Setup(s => s.SaveChanges()).ReturnsAsync(true);

            mockPersonService.Setup(s => s.EditPerson(It.IsAny<EditPersonDataDto>())).ReturnsAsync(true);

            var userService = new UserService(mockUserRepo.Object,
                                              mockPersonRepo.Object,
                                              mockPersonService.Object,
                                              mockAuthService.Object,
                                              mapper,
                                              converter);

            var result = await userService.EditUser(It.IsAny<UserViewDto>());

            Assert.IsTrue(result);
        }

        [Test]
        public async Task EditUser_GivenValidUserDataAndFailToUpdateUserEntityData_ShouldReturnFalse()
        {
            mockAuthService.Setup(s => s.EditRole(It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync(true);

            mockUserRepo.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                        .ReturnsAsync(GetUsers().First());
            mockUserRepo.Setup(s => s.Update(It.IsAny<ApplicationUser>()))
                        .Returns(Task.CompletedTask);
            mockUserRepo.Setup(s => s.SaveChanges()).ReturnsAsync(false);

            mockPersonService.Setup(s => s.EditPerson(It.IsAny<EditPersonDataDto>())).ReturnsAsync(true);

            var userService = new UserService(mockUserRepo.Object,
                                              mockPersonRepo.Object,
                                              mockPersonService.Object,
                                              mockAuthService.Object,
                                              mapper,
                                              converter);

            var result = await userService.EditUser(It.IsAny<UserViewDto>());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditUser_GivenValidUserDataAndFailToUpdateUserEntityDataAndPersonEntityData_ShouldReturnFalse()
        {
            mockAuthService.Setup(s => s.EditRole(It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync(true);

            mockUserRepo.Setup(s => s.GetSingleByCondition(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                        .ReturnsAsync(GetUsers().First());
            mockUserRepo.Setup(s => s.Update(It.IsAny<ApplicationUser>()))
                        .Returns(Task.CompletedTask);
            mockUserRepo.Setup(s => s.SaveChanges()).ReturnsAsync(false);

            mockPersonService.Setup(s => s.EditPerson(It.IsAny<EditPersonDataDto>())).ReturnsAsync(false);

            var userService = new UserService(mockUserRepo.Object,
                                              mockPersonRepo.Object,
                                              mockPersonService.Object,
                                              mockAuthService.Object,
                                              mapper,
                                              converter);

            var result = await userService.EditUser(It.IsAny<UserViewDto>());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetUserById_UserExists_ShouldReturnUserData()
        {
            mockUserRepo.Setup(s => s.GetUserById(It.IsAny<string>())).ReturnsAsync(GetUsers().First());

            mockPersonRepo.Setup(s => s.GetPersonWithChildEntities(It.IsAny<string>())).ReturnsAsync(person);

            var userService = new UserService(mockUserRepo.Object,
                                              mockPersonRepo.Object,
                                              mockPersonService.Object,
                                              mockAuthService.Object,
                                              mapper,
                                              converter);

            var result = await userService.GetUserById(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.NotNull(result.Person);
        }

        [Test]
        public async Task GetUserById_UserDoesNotExists_ShouldReturnNull()
        {
            mockUserRepo.Setup(s => s.GetUserById(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            var userService = new UserService(mockUserRepo.Object,
                                              mockPersonRepo.Object,
                                              mockPersonService.Object,
                                              mockAuthService.Object,
                                              mapper,
                                              converter);

            var result = await userService.GetUserById(It.IsAny<string>());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetUsers_ShouldReturnUsers()
        {
            mockUserRepo.Setup(s => s.GetUsers()).ReturnsAsync(GetUsers());

            mockUserRepo.Setup(s => s.GetUserRole(It.IsAny<string>()))
                        .ReturnsAsync(
                            new Microsoft.AspNetCore.Identity.IdentityRole
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "test"
                            });

            mockPersonRepo.Setup(s => s.GetPersonWithChildEntities(It.IsAny<string>()))
                          .ReturnsAsync(person);

            var userService = new UserService(mockUserRepo.Object,
                                              mockPersonRepo.Object,
                                              mockPersonService.Object,
                                              mockAuthService.Object,
                                              mapper,
                                              converter);

            var result = await userService.GetUsers();

            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(UserViewDto));
        }

        [Test]
        public async Task ResetUserPassword_GivenAnExistingUserId_ShouldResetPasswordAndReturnTrue()
        {
            mockAuthService.Setup(s => s.ResetPassword(It.IsAny<string>())).ReturnsAsync(true);

            var userService = new UserService(mockUserRepo.Object,
                                              mockPersonRepo.Object,
                                              mockPersonService.Object,
                                              mockAuthService.Object,
                                              mapper,
                                              converter);

            var result = await userService.ResetUserPassword(It.IsAny<string>());

            Assert.IsTrue(result);
        }

        [Test]
        public async Task ResetUserPassword_GivenAnUnexistingUserId_ShouldReturnFalse()
        {
            mockAuthService.Setup(s => s.ResetPassword(It.IsAny<string>())).ReturnsAsync(false);

            var userService = new UserService(mockUserRepo.Object,
                                              mockPersonRepo.Object,
                                              mockPersonService.Object,
                                              mockAuthService.Object,
                                              mapper,
                                              converter);

            var result = await userService.ResetUserPassword(It.IsAny<string>());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task DownloadUsersPdf_ShouldReturnByteArray()
        {
            mockUserRepo.Setup(s => s.GetUsers()).ReturnsAsync(GetUsers());

            mockUserRepo.Setup(s => s.GetUserRole(It.IsAny<string>()))
                        .ReturnsAsync(
                            new Microsoft.AspNetCore.Identity.IdentityRole
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "test"
                            });

            mockPersonRepo.Setup(s => s.GetPersonWithChildEntities(It.IsAny<string>()))
                          .ReturnsAsync(person);

            var userService = new UserService(mockUserRepo.Object,
                                              mockPersonRepo.Object,
                                              mockPersonService.Object,
                                              mockAuthService.Object,
                                              mapper,
                                              converter);

            var result = await userService.DownloadUsersPdf();

            CollectionAssert.IsNotEmpty(result);
        }

        [Test]
        public async Task DownloadUsersXlsl_ShouldReturnByteArray()
        {
            mockUserRepo.Setup(s => s.GetUsers()).ReturnsAsync(GetUsers());

            mockUserRepo.Setup(s => s.GetUserRole(It.IsAny<string>()))
                        .ReturnsAsync(
                            new Microsoft.AspNetCore.Identity.IdentityRole
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "test"
                            });

            mockPersonRepo.Setup(s => s.GetPersonWithChildEntities(It.IsAny<string>()))
                          .ReturnsAsync(person);

            var userService = new UserService(mockUserRepo.Object,
                                              mockPersonRepo.Object,
                                              mockPersonService.Object,
                                              mockAuthService.Object,
                                              mapper,
                                              converter);

            var result = await userService.DownloadUsersXlsl();

            CollectionAssert.IsNotEmpty(result);
        }

        public List<ApplicationUser> GetUsers()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();

            users.Add(new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                Email = "test@test.com",
                NormalizedEmail = "test@test.com",
                UserName = "test@test.com",
                NormalizedUserName = "test@test.com",
                Provider = "LOCAL"
            });

            users.Add(new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                Email = "test@test.com",
                NormalizedEmail = "test@test.com",
                UserName = "test@test.com",
                NormalizedUserName = "test@test.com",
                Provider = "LOCAL"
            });

            return users;
        }
    }
}
