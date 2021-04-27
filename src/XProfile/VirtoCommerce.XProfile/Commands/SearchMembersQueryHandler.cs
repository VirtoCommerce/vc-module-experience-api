using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class SearchMembersQueryHandler :
        IRequestHandler<SearchContactsQuery, MemberSearchResult>,
        IRequestHandler<SearchOrganizationsQuery, MemberSearchResult>
    {
        private readonly IMemberSearchService _memberSearchService;

        public SearchMembersQueryHandler(IMemberSearchService memberSearchService)
        {
            _memberSearchService = memberSearchService;
        }

        public Task<MemberSearchResult> Handle(SearchContactsQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = BuildMembersSearchCriteria(request, nameof(Contact));

            return _memberSearchService.SearchMembersAsync(searchCriteria);
        }

        public Task<MemberSearchResult> Handle(SearchOrganizationsQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = BuildMembersSearchCriteria(request, nameof(Organization));

            return _memberSearchService.SearchMembersAsync(searchCriteria);
        }

        private static MembersSearchCriteria BuildMembersSearchCriteria(SearchMembersQueryBase request, string memberType)
        {
            var result = AbstractTypeFactory<MembersSearchCriteria>.TryCreateInstance();
            result.MemberType = memberType;
            result.Keyword = request.Filter;
            result.Skip = request.Skip;
            result.Take = request.Take;
            result.Sort = request.Sort;

            return result;
        }
    }
}
