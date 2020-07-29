using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductAssociationsQueryHandler : IRequestHandler<SearchProductAssociationsQuery, SearchProductAssociationsResponse>
    {
        private readonly IProductAssociationSearchService _productAssociationSearchService;
        private readonly IMapper _mapper;
        public SearchProductAssociationsQueryHandler(IProductAssociationSearchService productAssociationSearchService, IMapper mapper)
        {
            _mapper = mapper;
            _productAssociationSearchService = productAssociationSearchService;
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
