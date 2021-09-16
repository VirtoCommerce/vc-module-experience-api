using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class PasswordValidationQuery : IQuery<IdentityResultResponse>
    {
        public string Password { get; set; }
    }
}
