using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetUserQuery : IQuery<ApplicationUser>
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string LoginProvider { get; }
        public string ProviderKey { get; }
        public string UserId { get; set; }

        public GetUserQuery()
        {

        }

        public GetUserQuery(string id = null, string email = null, string userName = null, string loginProvider = null, string providerKey = null)
        {
            Id = id;
            Email = email;
            UserName = userName;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }
    }
}
