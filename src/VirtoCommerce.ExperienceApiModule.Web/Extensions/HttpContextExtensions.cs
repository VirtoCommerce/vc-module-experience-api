using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
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
            if (principal != null)
            {
                if (!TryResolveTokenLoginOnBehalf(principal, ref operatorUserName))
                {
                    TryResolveLegacyLoginOnBehalf(context, ref principal, ref operatorUserName);
                }
            }

            var userContext = new GraphQLUserContext(principal);

            if (!string.IsNullOrEmpty(operatorUserName))
            {
                userContext.TryAdd("OperatorUserName", operatorUserName);
            }

            return userContext;
        }

        private static bool TryResolveTokenLoginOnBehalf(ClaimsPrincipal principal, ref string operatorUserName)
        {
            operatorUserName = principal.FindFirstValue(PlatformConstants.Security.Claims.OperatorUserName);

            return !string.IsNullOrEmpty(operatorUserName);
        }

        private static bool TryResolveLegacyLoginOnBehalf(HttpContext context, ref ClaimsPrincipal principal, ref string operatorUserName)
        {
            if (context.Request.Headers.TryGetValue("VirtoCommerce-User-Name", out var userNameFromHeader)
                            && principal.IsInRole(PlatformConstants.Security.SystemRoles.Administrator))
            {
                if (userNameFromHeader == "Anonymous")
                {
                    var identity = new ClaimsIdentity();

                    if (context.Request.Headers.TryGetValue("VirtoCommerce-Anonymous-User-Id", out var anonymousUserId))
                    {
                        identity.AddClaim(ClaimTypes.NameIdentifier, anonymousUserId);
                    }

                    principal = new ClaimsPrincipal(identity);
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

                return true;
            }

            return false;
        }
    }
}
