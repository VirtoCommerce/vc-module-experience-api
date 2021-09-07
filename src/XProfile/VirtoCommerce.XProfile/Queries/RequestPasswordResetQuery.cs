using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class RequestPasswordResetQuery : IQuery<bool>
    {
        public string LoginOrEmail { get; set; }

        public string CallbackUrl { get; set; }
    }
}
