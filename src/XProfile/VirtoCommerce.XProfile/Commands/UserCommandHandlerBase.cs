using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UserCommandHandlerBase
    {
        protected readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly AuthorizationOptions _securityOptions;

        public UserCommandHandlerBase(Func<UserManager<ApplicationUser>> userManager, IOptions<AuthorizationOptions> securityOptions)
        {
            _userManagerFactory = userManager; 
            _securityOptions = securityOptions.Value;
        }

        protected bool IsUserEditable(string userName)
        {
            return _securityOptions.NonEditableUsers?.FirstOrDefault(x => x.EqualsInvariant(userName)) == null;
        }
    }
}
