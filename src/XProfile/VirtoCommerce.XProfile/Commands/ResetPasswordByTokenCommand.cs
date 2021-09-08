using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class ResetPasswordByTokenCommand : ICommand<IdentityResultResponse>
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
