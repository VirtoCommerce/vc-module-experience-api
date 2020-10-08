using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class ExpInventorySearchService : IExpInventorySearchService, IService
    {
        private readonly IInventorySearchService _inventorySearchService;

        public ExpInventorySearchService(IInventorySearchService inventorySearchService)
        {
            _inventorySearchService = inventorySearchService;
        }

        public string Provider { get; set; } = Providers.PlatformModule;

        public async Task<SearchProductResponse> SearchInventoriesAsync(SearchProductResponse parameter)
        {
            var query = parameter.Query;
            if (query == null)
            {
                throw new OperationCanceledException("Query must be set");
            }

            var productIds = parameter.Results.Select(x => x.Id).ToArray();
            var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
            // If products availabilities requested
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadInventories))
            {
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
            }

            return parameter;
        }
    }
}
