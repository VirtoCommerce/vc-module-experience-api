using System;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    [Obsolete("Move required properties from ApplicationUser")]
    public class CreateUserCommand : ApplicationUser, ICommand<IdentityResult>
    {
        public string UserId { get; set; }
    }
}
