using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nest;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Requests;

namespace VirtoCommerce.ExperienceApiModule.Data.Handlers
{
    public class LoadProductsRequestHandler : IRequestHandler<LoadProductRequest, LoadProductResponse>
    {
        private readonly IProductIndexedSearchService _productIndexedSearchService;
        public LoadProductsRequestHandler(IProductIndexedSearchService productIndexedSearchService)
        {
            _productIndexedSearchService = productIndexedSearchService;
        }

        public virtual async Task<LoadProductResponse> Handle(LoadProductRequest request, CancellationToken cancellationToken)
        {
            var result = new LoadProductResponse();
            var criteria = new ProductIndexedSearchCriteria
            {                
                ObjectIds = request.Ids,
                IncludeFields = request.IncludeFields.Select(x => "__object." + x).ToArray(),
                Take = request.Ids.Count()
            };
            var searchResult = await _productIndexedSearchService.SearchAsync(criteria);
            result.Products = searchResult.Items.ToList();

           return result;
        }
    }
}
