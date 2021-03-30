using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectBoss.Data.DatabaseContext;

namespace ProjectBoss.Api.Configuration
{
    public static class DbContextConfiguration
    {
        public static IServiceCollection AddMainDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opts => {
                opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                opts.EnableSensitiveDataLogging();
            });

            return services;
        }
    }
}
