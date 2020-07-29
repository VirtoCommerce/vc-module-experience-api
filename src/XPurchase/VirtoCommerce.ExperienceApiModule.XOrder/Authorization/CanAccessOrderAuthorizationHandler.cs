using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Authorization
{
    public sealed class CanAccessOrderAuthorizationRequirement : PermissionAuthorizationRequirement
    {
        public CanAccessOrderAuthorizationRequirement() : base("CanAccessOrder")
        {
        }
    }

    public class CanAccessOrderAuthorizationHandler : PermissionAuthorizationHandlerBase<CanAccessOrderAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessOrderAuthorizationRequirement requirement)
        {
            var result = false;
            if (context.Resource is CustomerOrder[] orders)
            {
                //TODO use ClaimTypes instead of "name"
                result = orders.All(x => x.CustomerId == context.User.FindFirstValue("name"));
            }

            if (result)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
