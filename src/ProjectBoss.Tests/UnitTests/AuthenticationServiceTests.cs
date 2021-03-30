using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using ProjectBoss.Api.Configuration.Extensions;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Services;
using ProjectBoss.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectBoss.Tests.UnitTests
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IUserStore<ApplicationUser>> mockUserManager;
        private readonly Mock<IRoleStore<IdentityRole>> mockRoleManager;
        private readonly IOptions<AppSettings> options;
    }
}
