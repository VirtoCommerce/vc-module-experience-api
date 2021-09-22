using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class SearchMembersQuery : IQuery<MemberSearchResult>
    {
        public string MemberId { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Sort { get; set; }
        public string SearchPhrase { get; set; }
        public string MemberType { get; set; }
        public IList<string> ObjectIds { get; set; }
    }
}
