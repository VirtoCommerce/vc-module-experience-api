using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetUserByEmailQuery : IQuery<ApplicationUser>
    {
        public string Email { get; set; }

        public GetUserByEmailQuery()
        {

        }

        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }
}
