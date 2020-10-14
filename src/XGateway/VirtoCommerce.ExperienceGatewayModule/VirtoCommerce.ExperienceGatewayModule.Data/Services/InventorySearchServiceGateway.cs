using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.XGateway.Core.Models;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule.Data.Services
{
    public class InventorySearchServiceGateway : IInventorySearchServiceGateway
    {
        private readonly IInventorySearchService _inventorySearchService;

        public InventorySearchServiceGateway(IInventorySearchService inventorySearchService)
        {
            _inventorySearchService = inventorySearchService;
        }

        public string Gateway { get; set; } = Gateways.VirtoCommerce;

        public Task<InventoryInfoSearchResult> SearchInventoriesAsync(InventorySearchCriteria criteria)
        {
            return _inventorySearchService.SearchInventoriesAsync(criteria);
        }
    }
}
