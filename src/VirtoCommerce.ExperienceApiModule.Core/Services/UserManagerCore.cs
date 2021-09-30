using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.Core.Services
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
    }
}
