using MediatR;
using VirtoCommerce.CustomerModule.Core.Model.Search;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public class SearchOrganizationMembersQuery : IRequest<MemberSearchResult>
    {
        public string OrganizationId { get; set; }

        public string UserId { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }
        public string Sort { get; set; }
        public string SearchPhrase { get; set; }
    }
}
