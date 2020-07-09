using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetUserByIdQuery : IQuery<ApplicationUser>
    {
        public string Id { get; set; }

        public GetUserByIdQuery()
        {

        }

        public GetUserByIdQuery(string id)
        {
            Id = id;
        }
    }
}
