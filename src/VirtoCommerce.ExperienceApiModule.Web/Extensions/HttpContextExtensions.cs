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

            // Impersonate a user based on their VC account object id by passing that value along with the header VirtoCommerce-User-Name.
            if (principal != null
                && context.Request.Headers.TryGetValue("VirtoCommerce-User-Name", out var userNameFromHeader)
                && principal.IsInRole(PlatformConstants.Security.SystemRoles.Administrator))
            {
                if (userNameFromHeader == "Anonymous")
                {
                    // create principal for anon user
                    principal = new ClaimsPrincipal(new ClaimsIdentity());
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
                }
            }
            return new GraphQLUserContext(principal);
        }
    }
}
