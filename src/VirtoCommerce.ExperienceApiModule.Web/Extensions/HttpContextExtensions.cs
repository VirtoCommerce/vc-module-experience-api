using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.Web.Extensions
{
    public static class HttpContextExtensions
    {
        public static GraphQLUserContext BuildGraphQLUserContext(this HttpContext context)
        {
            var principal = context.User;
            var operatorUserName = default(string);

            // Impersonate a user based on their VC account object id by passing that value along with the header VirtoCommerce-User-Name.
            if (principal != null
                && context.Request.Headers.TryGetValue("VirtoCommerce-User-Name", out var userNameFromHeader)
                && principal.IsInRole(PlatformConstants.Security.SystemRoles.Administrator))
            {
                if (userNameFromHeader == "Anonymous")
                {
                    if (context.Request.Headers.TryGetValue("VirtoCommerce-AnonymousUser-Id", out var anonymousUserId))
                    {
                        var userIdClaim = new Claim(ClaimTypes.NameIdentifier, anonymousUserId);
                        var identity = new ClaimsIdentity(new[] { userIdClaim }, null);
                        principal = new ClaimsPrincipal(identity);
                    }
                    else
                    {
                        // create principal for anonymous user identity
                        principal = new ClaimsPrincipal(new ClaimsIdentity());
                    }
                }
                else
                {
                    var factory = context.RequestServices.GetService<Func<SignInManager<ApplicationUser>>>();
                    var signInManager = factory();
                    var user = signInManager.UserManager.FindByNameAsync(userNameFromHeader).GetAwaiter().GetResult();
                    if (user != null)
                    {
                        principal = signInManager.CreateUserPrincipalAsync(user).GetAwaiter().GetResult();
                    }

                    // try to find LoginOnBehalf operator, if VirtoCommerce-Operator-User-Name header is present
                    if (context.Request.Headers.TryGetValue("VirtoCommerce-Operator-User-Name", out var operatorUserNameHeader))
                    {
                        operatorUserName = operatorUserNameHeader;
                    }
                }
            }

            var userContext = new GraphQLUserContext(principal);

            if (!string.IsNullOrEmpty(operatorUserName))
            {
                userContext.TryAdd("OperatorUserName", operatorUserName);
            }

            return userContext;
        }
    }
}
