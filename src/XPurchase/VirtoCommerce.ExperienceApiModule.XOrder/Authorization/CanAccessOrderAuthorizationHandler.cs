using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
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
        private readonly IMemberService _memberService;

        public CanAccessOrderAuthorizationHandler(IMemberService memberService)
        {
            _memberService = memberService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessOrderAuthorizationRequirement requirement)
        {
            var result = context.User.IsInRole(PlatformConstants.Security.SystemRoles.Administrator);

            if (!result)
            {
                if (context.Resource is CustomerOrder order)
                {
                    result = order.CustomerId == GetUserId(context);
                }
                else if (context.Resource is SearchCustomerOrderQuery query)
                {
                    query.CustomerId = GetUserId(context);
                    result = query.CustomerId != null;
                }
                else if (context.Resource is SearchOrganizationOrderQuery organizationOrderQuery)
                {
                    if (!string.IsNullOrEmpty(organizationOrderQuery.OrganizationId))
                    {
                        var memberId = GetMemberId(context);
                        var member = await _memberService.GetByIdAsync(memberId);
                        result = MemberAssignedToOrganization(member, organizationOrderQuery.OrganizationId);
                    }
                }
                else if (context.Resource is SearchPaymentsQuery paymentsQuery)
                {
                    paymentsQuery.CustomerId = GetUserId(context);
                    result = paymentsQuery.CustomerId != null;
                }
                else if (context.Resource is ShoppingCart cart)
                {
                    var currentUserId = GetUserId(context);
                    result = cart.CustomerId == currentUserId || (currentUserId == null && cart.IsAnonymous);
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
        }

        private static string GetUserId(AuthorizationHandlerContext context)
        {
            //PT-5375 use ClaimTypes instead of "name"
            return context.User.FindFirstValue("name");
        }

        private static string GetMemberId(AuthorizationHandlerContext context)
        {
            return context.User.FindFirstValue("memberId");
        }

        public static bool MemberAssignedToOrganization(Member member, string organizationId)
        {
            return member?.MemberType switch
            {
                nameof(Contact) => (member as Contact)?.Organizations?.Contains(organizationId) ?? false,
                nameof(Employee) => (member as Employee)?.Organizations?.Contains(organizationId) ?? false,
                _ => false
            };
        }
    }
}
