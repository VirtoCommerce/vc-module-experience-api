using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class PasswordValidationQuery : IQuery<PasswordValidationResponse>
    {
        public string Password { get; set; }
    }
}
