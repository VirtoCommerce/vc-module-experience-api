using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.XGateway.Core.Models;
using VirtoCommerce.XGateway.Core.Services;


namespace VirtoCommerce.ExperienceGatewayModule.Data.Services
{
    public class ProductAssociationSearchServiceGateway : IProductAssociationSearchServiceGateway
    {
        private readonly IProductAssociationSearchService _productAssociationSearchService;

        public ProductAssociationSearchServiceGateway(IProductAssociationSearchService productAssociationSearchService)
        {
            _productAssociationSearchService = productAssociationSearchService;
        }

        public string Gateway { get; set; } = Gateways.VirtoCommerce;

        public Task<ProductAssociationSearchResult> SearchProductAssociationsAsync(ProductAssociationSearchCriteria criteria)
        {
            return _productAssociationSearchService.SearchProductAssociationsAsync(criteria);
        }
    }
}
