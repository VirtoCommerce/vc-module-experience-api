using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class SearchOrganizationMembersQuery : IQuery<MemberSearchResult>
    {
        public string OrganizationId { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Sort { get; set; }
        public string SearchPhrase { get; set; }
        public string UserId { get; set; }
    }
}
