using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class ResetPasswordByTokenCommand : ICommand<IdentityResult>
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
