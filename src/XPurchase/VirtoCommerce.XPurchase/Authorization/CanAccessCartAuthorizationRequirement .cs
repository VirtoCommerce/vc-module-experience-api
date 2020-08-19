using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.XPurchase.Queries;

namespace VirtoCommerce.XPurchase.Authorization
{
    public class CanAccessCartAuthorizationRequirement: PermissionAuthorizationRequirement
    {
        public CanAccessCartAuthorizationRequirement() : base("CanAccessCart")
        {

        }
    }

    public class CanAccessCartAuthorizationHandler : PermissionAuthorizationHandlerBase<CanAccessCartAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessCartAuthorizationRequirement requirement)
        {
            var result = false;
            if (context.Resource is ShoppingCart cart)
            {
                result = cart.CustomerId == GetUserId(context);
            }
            else if (context.Resource is SearchCartQuery searchQuery)
            {
                searchQuery.UserId = GetUserId(context);
                result = true;
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
