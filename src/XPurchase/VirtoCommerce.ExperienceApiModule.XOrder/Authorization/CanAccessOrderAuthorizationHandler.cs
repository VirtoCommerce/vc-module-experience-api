using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core;
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

            var result = context.User.IsInRole(PlatformConstants.Security.SystemRoles.Administrator);

            if (!result)
            {
                if (context.Resource is CustomerOrder order)
                {
                    result = order.CustomerId == GetUserId(context);
                }
                else if (context.Resource is SearchOrderQuery query)
                {
                    query.CustomerId = GetUserId(context);
                    result = query.CustomerId != null; 
                }
                else if (context.Resource is SearchPaymentsQuery paymentsQuery)
                {
                    paymentsQuery.CustomerId = GetUserId(context);
                    result = paymentsQuery.CustomerId != null;
                }
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

        private static string GetUserId(AuthorizationHandlerContext context)
        {
            //TODO use ClaimTypes instead of "name"
            return context.User.FindFirstValue("name");
        }
    }
}
