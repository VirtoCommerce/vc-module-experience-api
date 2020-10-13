using System.Threading.Tasks;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public interface IInventorySearchServiceGateway
    {
        Task<SearchProductResponse> SearchInventoriesAsync(SearchProductResponse query);
    }
}
