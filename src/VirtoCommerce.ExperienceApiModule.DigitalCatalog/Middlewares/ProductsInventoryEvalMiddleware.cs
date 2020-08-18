using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class ProductsInventoryEvalMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IInventorySearchService _inventorySearchService;

        public ProductsInventoryEvalMiddleware(IInventorySearchService inventorySearchService)
        {
            _inventorySearchService = inventorySearchService;
        }

        public async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var query = parameter.Query;
            if (query == null)
            {
                throw new OperationCanceledException("Query must be set");
            }

            var productIds = parameter.Results.Select(x => x.Id).ToArray();
            // If products availabilities requested
            if (query.HasInventoryFields())
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


            await next(parameter);
        }


    }
}
