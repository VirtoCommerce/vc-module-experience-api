using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public class LockUserCommand : UserCommandBase<IdentityResult>
    {
    }
}
