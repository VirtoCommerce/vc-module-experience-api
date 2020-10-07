using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class ExpProductAssociationSearchService : IExpProductAssociationSearchService, IService
    {
        private readonly IProductAssociationSearchService _productAssociationSearchService;
        private readonly IMapper _mapper;

        public ExpProductAssociationSearchService(IProductAssociationSearchService productAssociationSearchService, IMapper mapper)
        {
            _productAssociationSearchService = productAssociationSearchService;
            _mapper = mapper;
        }

        public string Provider { get; set; } = Providers.PlatformModule;

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
