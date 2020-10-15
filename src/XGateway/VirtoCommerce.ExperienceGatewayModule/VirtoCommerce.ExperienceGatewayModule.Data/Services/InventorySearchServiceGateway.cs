using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule.Data.Services
{
    public class InventorySearchServiceGateway : IInventorySearchServiceGateway
    {
        private readonly IInventorySearchService _inventorySearchService;

        public InventorySearchServiceGateway(/*IInventorySearchService inventorySearchService*/)
        {
            //_inventorySearchService = inventorySearchService;
        }

        public Task<InventoryInfoSearchResult> SearchInventoriesAsync(InventorySearchCriteria criteria)
        {
            return _inventorySearchService.SearchInventoriesAsync(criteria);
        }
    }
}
