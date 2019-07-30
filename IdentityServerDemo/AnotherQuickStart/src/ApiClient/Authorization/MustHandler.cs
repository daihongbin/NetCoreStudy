using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClient.Authorization
{
    public class MustHandler : AuthorizationHandler<MustRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustRequirement requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            if (filterContext == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var gender = context.User.Claims.FirstOrDefault(f => f.Type == "gender").Value;
            var nationality = context.User.Claims.FirstOrDefault(f => f.Type == "nationality").Value;

            if (gender == "female" && nationality == "China")
            {
                context.Succeed(requirement);
            }

            if (gender == "male" && nationality == "USA")
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
