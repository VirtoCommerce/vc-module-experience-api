using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class SearchMembersQueryHandler : IRequestHandler<SearchMembersQuery, MemberSearchResult>
    {
        private readonly IMemberSearchService _memberSearchService;

        public SearchMembersQueryHandler(IMemberSearchService memberSearchService)
        {
            _memberSearchService = memberSearchService;
        }

        public virtual async Task<MemberSearchResult> Handle(SearchMembersQuery request, CancellationToken cancellationToken)
        {
            var criteria = new MembersSearchCriteria
            {
                MemberId = request.MemberId,
                SearchPhrase = request.SearchPhrase,
                Skip = request.Skip,
                Take = request.Take,
                Sort = request.Sort,
                MemberType = request.MemberType,
                ObjectIds = request.ObjectIds,
            };

            var result = await _memberSearchService.SearchMembersAsync(criteria);

            return result;
        }
    }
}
