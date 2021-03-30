using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Extensions
{
    public class MutiplePoliciesAuthorizeAttribute : TypeFilterAttribute
    {
        public MutiplePoliciesAuthorizeAttribute(string policies, bool isAnd = false) : base(typeof(MultiplePoliciesAuthorizeFilter))
        {
            Arguments = new object[] { policies, isAnd };
        }
    }

    public class MultiplePoliciesAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IWebHostEnvironment environment;

        public string policies { get; private set; }
        public bool isAnd { get; private set; }

        public MultiplePoliciesAuthorizeFilter(string _policies, bool _isAnd, IAuthorizationService _authorizationService, IWebHostEnvironment _environment)
        {
            authorizationService = _authorizationService;
            environment = _environment;

            policies = _policies;
            isAnd = _isAnd;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (environment.EnvironmentName == "IntegrationTest")
                return;

            var pols = policies.Split(";").ToList();
            if (isAnd)
            {
                foreach (var pol in pols)
                {
                    var authorized = await authorizationService.AuthorizeAsync(context.HttpContext.User, pol);
                    if (!authorized.Succeeded)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }
            else
            {
                foreach (var pol in pols)
                {
                    var authorized = await authorizationService.AuthorizeAsync(context.HttpContext.User, pol);
                    if (authorized.Succeeded)
                        return;
                }
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
