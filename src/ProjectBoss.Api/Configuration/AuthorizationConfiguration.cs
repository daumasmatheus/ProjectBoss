using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBoss.Api.Configuration
{
    public static class AuthorizationConfiguration
    {
        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("RequireUser", pol => pol.RequireRole("User"));
                opts.AddPolicy("RequireProjectManager", pol => pol.RequireRole("Project Manager"));
                opts.AddPolicy("RequireAdministrator", pol => pol.RequireRole("Administrator"));
            });

            return services;
        }
    }
}
