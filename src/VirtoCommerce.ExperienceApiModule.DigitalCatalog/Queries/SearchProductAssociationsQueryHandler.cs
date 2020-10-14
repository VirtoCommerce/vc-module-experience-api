using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductAssociationsQueryHandler : IRequestHandler<SearchProductAssociationsQuery, SearchProductAssociationsResponse>
    {
        private readonly IProductAssociationSearchServiceGateway _productAssociationSearchService;
        private readonly IMapper _mapper;

        public SearchProductAssociationsQueryHandler(IProductAssociationSearchServiceGateway productAssociationSearchService, IMapper mapper)
        {
            _productAssociationSearchService = productAssociationSearchService;
            _mapper = mapper;
        }

        public async Task<SearchProductAssociationsResponse> Handle(SearchProductAssociationsQuery request, CancellationToken cancellationToken)
        {
            var result = await _productAssociationSearchService.SearchProductAssociationsAsync(_mapper.Map<ProductAssociationSearchCriteria>(request));

            return new SearchProductAssociationsResponse
            {
                Result = result
            };
        }
    }
}
