using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Contracts;

namespace VirtoCommerce.ExperienceApiModule.Data.Handlers
{
    public class SearchProductRequestHandler : IRequestHandler<SearchProductRequest, SearchProductResponse>
    {
        private readonly IProductSearchService _searchService;
        public SearchProductRequestHandler(IProductSearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<SearchProductResponse> Handle(SearchProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _searchService.SearchProductsAsync(request.Criteria);
            return new SearchProductResponse {  Result = result };
        }
    }
}
