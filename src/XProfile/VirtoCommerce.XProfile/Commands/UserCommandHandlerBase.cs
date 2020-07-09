using System;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UserCommandHandlerBase
    {
        protected readonly IServiceProvider _services;
        private readonly AuthorizationOptions _securityOptions;

        public UserCommandHandlerBase(IServiceProvider services, IOptions<AuthorizationOptions> securityOptions)
        {
            _services = services;
            _securityOptions = securityOptions.Value;
        }

        protected bool IsUserEditable(string userName)
        {
            return _securityOptions.NonEditableUsers?.FirstOrDefault(x => x.EqualsInvariant(userName)) == null;
        }
    }
}
