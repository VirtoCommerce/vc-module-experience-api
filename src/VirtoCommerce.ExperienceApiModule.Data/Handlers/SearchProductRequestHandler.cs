using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core;
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

        public virtual async Task<SearchProductResponse> Handle(SearchProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _searchService.SearchProductsAsync(request.Criteria);
            foreach (var product in result.Results.OfType<CatalogProduct2>())
            {
                product.DataSource = "Virto";
            }
            return new SearchProductResponse {  Result = result };
        }
    }
}
