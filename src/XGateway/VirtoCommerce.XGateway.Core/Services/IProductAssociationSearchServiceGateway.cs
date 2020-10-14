using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.XGateway.Core.Services
{
    public interface IProductAssociationSearchServiceGateway : IServiceGateway
    {
        Task<ProductAssociationSearchResult> SearchProductAssociationsAsync(ProductAssociationSearchCriteria criteria);
    }
}
