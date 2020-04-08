using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Requests;

namespace VirtoCommerce.ExperienceApiModule.Data.Handlers
{
    public class SearchProductAssociationsHandler : IRequestHandler<SearchProductAssociationsRequest, SearchProductAssociationsResponse>
    {
        private readonly IProductAssociationSearchService _searchService;
        public SearchProductAssociationsHandler(IProductAssociationSearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<SearchProductAssociationsResponse> Handle(SearchProductAssociationsRequest request, CancellationToken cancellationToken)
        {
            var result = await _searchService.SearchProductAssociationsAsync(request.Criteria);
            return new SearchProductAssociationsResponse {  Result = result };
        }
    }
}
