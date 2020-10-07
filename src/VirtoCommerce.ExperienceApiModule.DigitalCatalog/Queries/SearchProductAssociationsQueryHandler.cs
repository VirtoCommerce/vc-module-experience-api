using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.XDigitalCatalog.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductAssociationsQueryHandler : IRequestHandler<SearchProductAssociationsQuery, SearchProductAssociationsResponse>
    {
        private readonly IExpProductAssociationSearchService _productAssociationSearchService;

        public SearchProductAssociationsQueryHandler(IExpProductAssociationSearchService productAssociationSearchService)
        {
            _productAssociationSearchService = productAssociationSearchService;
        }

        public Task<SearchProductAssociationsResponse> Handle(SearchProductAssociationsQuery request, CancellationToken cancellationToken)
        {
            return _productAssociationSearchService.SearchProductAssociationsAsync(request);
        }
    }
}
