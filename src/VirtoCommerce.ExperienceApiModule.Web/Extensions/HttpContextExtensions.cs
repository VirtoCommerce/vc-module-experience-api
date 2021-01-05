using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core;

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
                    principal = null;
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
