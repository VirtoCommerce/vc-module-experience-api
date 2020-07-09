using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class SearchOrganizationMembersQueryHandler : IRequestHandler<SearchOrganizationMembersQuery, MemberSearchResult>
    {
        private readonly IMemberSearchService _memberSearchService;

        public SearchOrganizationMembersQueryHandler(IMemberSearchService memberSearchService)
        {
            _memberSearchService = memberSearchService;
        }

        public async Task<MemberSearchResult> Handle(SearchOrganizationMembersQuery request, CancellationToken cancellationToken)
        {
            var criteria = new MembersSearchCriteria
            {
                MemberId = request.OrganizationId,
                SearchPhrase = request.SearchPhrase,
                Skip = request.Skip,
                Take = request.Take,
                Sort = request.Sort,
                MemberType = nameof(Contact)
            };
            return await _memberSearchService.SearchMembersAsync(criteria);
        }
    }
}
