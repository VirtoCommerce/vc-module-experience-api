using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.Data.Services
{
    public class UserManagerCore : IUserManagerCore
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public UserManagerCore(Func<UserManager<ApplicationUser>> userManagerFactory)
        {
            _userManagerFactory = userManagerFactory;
        }

        public virtual async Task<bool> IsLockedOutAsync(ApplicationUser user)
        {
            using var userManager = _userManagerFactory();

            var result = await userManager.IsLockedOutAsync(user);

            return result;
        }

        [Obsolete("Use CheckUserState(string userId, bool allowAnonymous)", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public Task CheckUserState(string userId)
        {
            return CheckUserState(userId, allowAnonymous: true);
        }

        public async Task CheckUserState(string userId, bool allowAnonymous)
        {
            var userManager = _userManagerFactory();
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                if (allowAnonymous)
                {
                    return;
                }

                throw AuthorizationError.AnonymousAccessDenied();
            }

            if (user.PasswordExpired)
            {
                throw AuthorizationError.PasswordExpired();
            }

            if (await userManager.IsLockedOutAsync(user))
            {
                throw AuthorizationError.UserLocked();
            }
        }
    }
}
