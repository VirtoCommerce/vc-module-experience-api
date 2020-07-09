using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetUserByNameQuery : IQuery<ApplicationUser>
    {
        public string UserName { get; set; }

        public GetUserByNameQuery()
        {

        }

        public GetUserByNameQuery(string userName)
        {
            UserName = userName;
        }
    }
}
