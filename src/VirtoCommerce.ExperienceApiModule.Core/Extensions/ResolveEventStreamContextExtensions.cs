using System.Security.Claims;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Subscription;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class ResolveEventStreamContextExtensions
    {
        public static string GetCurrentUserId(this IResolveEventStreamContext resolveContext)
        {
            var claimsPrincipal = GetCurrentPrincipal(resolveContext);
            return claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? claimsPrincipal?.FindFirstValue("name") ?? AnonymousUser.UserName;
        }

        public static ClaimsPrincipal GetCurrentPrincipal(this IResolveEventStreamContext resolveContext)
        {
            var messageContext = ((MessageHandlingContext)resolveContext.UserContext);

            if (messageContext.TryGetValue("User", out var value))
            {
                var claimsPrincipal = value as ClaimsPrincipal;
                return claimsPrincipal;
            }

            return null;
        }
    }
}
