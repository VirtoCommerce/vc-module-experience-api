using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateUserCommand : ApplicationUser, ICommand<IdentityResult>
    {
        public string UserId { get; set; }
    }
}
