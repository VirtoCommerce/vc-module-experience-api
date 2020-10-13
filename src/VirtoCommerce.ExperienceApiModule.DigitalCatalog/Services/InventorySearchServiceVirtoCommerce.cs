using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class InventorySearchServiceVirtoCommerce : IInventorySearchServiceGateway
    {
        private readonly IInventorySearchService _inventorySearchService;

        public InventorySearchServiceVirtoCommerce(IInventorySearchService inventorySearchService)
        {
            _inventorySearchService = inventorySearchService;
        }

        public string Gateway { get; set; } = ExperienceApiModule.Core.Models.Gateways.VirtoCommerce;

        public async Task<SearchProductResponse> SearchInventoriesAsync(SearchProductResponse parameter)
        {
            var productIds = parameter.Results.Select(x => x.Id).ToArray();
            var inventories = await _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria
            {
                ProductIds = productIds,
                //Do not use int.MaxValue use only 10 items per requested product
                //TODO: Replace to pagination load
                Take = Math.Min(productIds.Length * 10, 500)
            });
            if (inventories.Results.Any())
            {
                parameter.Results.Apply(x => x.ApplyStoreInventories(inventories.Results, parameter.Store));
            }

            return parameter;
        }
    }
}
