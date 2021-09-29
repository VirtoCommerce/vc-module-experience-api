using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateUserCommand : ICommand<IdentityResult>
    {
        public ApplicationUser ApplicationUser { get; set; } = new ApplicationUser();
    }
}
