using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Requests;

namespace VirtoCommerce.ExperienceApiModule.Data.Handlers
{
    public class SearchProductRequestHandler : IRequestHandler<SearchProductRequest, SearchProductResponse>
    {
        private readonly IProductIndexedSearchService _productIndexedSearchService;
        public SearchProductRequestHandler(IProductIndexedSearchService productIndexedSearchService)
        {
            _productIndexedSearchService = productIndexedSearchService;
        }

        public virtual async Task<SearchProductResponse> Handle(SearchProductRequest request, CancellationToken cancellationToken)
        {
            var result = new SearchProductResponse();
       
            var searchResult = await _productIndexedSearchService.SearchAsync(request.Criteria);
            result.Result.Results = searchResult.Items.ToList();
            result.Result.TotalCount = (int)searchResult.TotalCount;
            return result;
        }
    }
}
