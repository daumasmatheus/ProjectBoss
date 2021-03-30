using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.DependencyInjection;
using ProjectBoss.Api.Services;
using ProjectBoss.Api.Services.Interfaces;
using ProjectBoss.Data.Repositories;
using ProjectBoss.Data.Repositories.Interfaces;

namespace ProjectBoss.Api.Configuration
{
    public static class DependencyInjectionContainer
    {
        public static IServiceCollection AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IPersonInProjectRepository, PersonInProjectRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IPersonInProjectService, PersonInProjectService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
