using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace ProjectBoss.Api.Controllers.Base
{
    [ApiController]
    public class BaseController : Controller
    {
        protected ICollection<string> Errors = new List<string>();

        protected IActionResult CustomResponse(object result = null)
        {
            if (ValidOperation())
                return Ok(result);

            return BadRequest(
                new ValidationProblemDetails(new Dictionary<string, string[]> {
                    { "Messages", Errors.ToArray() }
                })
            );
        }                

        protected IActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
                AddProccessError(error.ErrorMessage);

            return CustomResponse();
        }

        protected bool ValidOperation()
            => !Errors.Any();
        protected void AddProccessError(string error)
            => Errors.Add(error);
        protected void ClearProccessErrors()
            => Errors.Clear();
        protected ActionResult HandleException(string message)
        {
            var currentUser = HttpContext.User?.Identity?.Name ?? "No user";            

            return BadRequest($"message: {message}; route: {ControllerContext.ActionDescriptor.AttributeRouteInfo.Template}; user: {currentUser}");
        }        
    }
}
