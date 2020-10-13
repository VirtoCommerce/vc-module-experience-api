using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class ProductAssociationSearchServiceVirtoCommerce : IProductAssociationSearchServiceGateway
    {
        private readonly IProductAssociationSearchService _productAssociationSearchService;
        private readonly IMapper _mapper;

        public ProductAssociationSearchServiceVirtoCommerce(IProductAssociationSearchService productAssociationSearchService, IMapper mapper)
        {
            _productAssociationSearchService = productAssociationSearchService;
            _mapper = mapper;
        }

        public string Gateway { get; set; } = ExperienceApiModule.Core.Models.Gateways.VirtoCommerce;

        public async Task<SearchProductAssociationsResponse> SearchProductAssociationsAsync(SearchProductAssociationsQuery request)
        {
            var result = await _productAssociationSearchService.SearchProductAssociationsAsync(_mapper.Map<ProductAssociationSearchCriteria>(request));

            return new SearchProductAssociationsResponse
            {
                Result = result
            };
        }
    }
}
