using System.Threading.Tasks;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public interface IExpInventorySearchService
    {
        Task<SearchProductResponse> SearchInventoriesAsync(SearchProductResponse query);
    }
}
