using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjectBoss.Api.Configuration;
using ProjectBoss.Data.DatabaseContext;
using ProjectBoss.Domain.Extensions;
using System;

namespace ProjectBoss.Tests.IntegrationTests.Base
{
    public class IntegrationTestsBase
    {
        protected ApplicationDbContext dbContext;
        protected IMapper mapper;

        protected Guid ADMIN_USER_ID = Guid.Parse("129CEA46-F5C8-424F-86CB-FB3391308889");
        protected Guid ADMIN_PERSON_ID = Guid.Parse("A3FD4B32-927D-4538-B6F5-29CE6FD717BD");

        protected Guid USER2_USER_ID = Guid.Parse("F6A3FD2A-CD5B-4BAB-AD79-8EF374F0C92E");
        protected Guid USER2_PERSON_ID = Guid.Parse("0905E86B-B95E-4F43-9282-65F584F8F5F5");

        public IntegrationTestsBase()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
                                                                         .AddJsonFile("appsettings.json")
                                                                         .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(ConfigurationExtensions.GetConnectionString(configuration, "DefaultConnection"));
            dbContext = new ApplicationDbContext(builder.Options);

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var user = new ApplicationUser
            {
                Id = USER2_USER_ID.ToString(),
                PasswordHash = "AQAAAAEAACcQAAAAEPMDjNaAOUgXCoM6Pvp++mnypj0VncQAXPxf7F4gi+tirnRyFGidhylYRuMscXJ/QA==",
                EmailConfirmed = true,
                Email = "user2@pjb.com",
                NormalizedEmail = "user2@pjb.com",
                UserName = "user2@pjb.com",
                NormalizedUserName = "user2@pjb.com"
            };
            dbContext.User.Add(user);

            var person = new Core.Entities.Person
            {
                PersonId = USER2_PERSON_ID,
                UserId = USER2_USER_ID.ToString(),
                FirstName = "User2",
                LastName = "ProjectBoss"
            };
            dbContext.Person.Add(person);

            mapper = new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper();
        }
    }
}
