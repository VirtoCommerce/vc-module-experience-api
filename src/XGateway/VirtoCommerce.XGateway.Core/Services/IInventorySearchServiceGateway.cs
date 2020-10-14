using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model.Search;

namespace VirtoCommerce.XGateway.Core.Services
{
    public interface IInventorySearchServiceGateway : IServiceGateway
    {
        Task<InventoryInfoSearchResult> SearchInventoriesAsync(InventorySearchCriteria criteria);
    }
}
