using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Search;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductAssociationsQueryHandler : IRequestHandler<SearchProductAssociationsQuery, SearchProductAssociationsResponse>
    {
        private readonly IProductAssociationSearchService _productAssociationSearchService;

        public SearchProductAssociationsQueryHandler(IProductAssociationSearchService productAssociationSearchService)
        {
            _productAssociationSearchService = productAssociationSearchService;
        }

        public async Task<SearchProductAssociationsResponse> Handle(SearchProductAssociationsQuery request, CancellationToken cancellationToken)
        {
            var result = await _productAssociationSearchService.SearchProductAssociationsAsync(request.Criteria);

            return new SearchProductAssociationsResponse
            {
                Result = result
            };
        }
    }
}
