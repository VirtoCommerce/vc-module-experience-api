using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetUserByLoginQuery : IQuery<ApplicationUser>
    {
        public string LoginProvider { get; }
        public string ProviderKey { get; }

        public GetUserByLoginQuery()
        {

        }

        public GetUserByLoginQuery(string loginProvider, string providerKey)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }
    }
}
