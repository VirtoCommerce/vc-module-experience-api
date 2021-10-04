using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsInventoryMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IInventorySearchService _inventorySearchService;

        public EvalProductsInventoryMiddleware(IInventorySearchService inventorySearchService)
        {
            _inventorySearchService = inventorySearchService;
        }

        public virtual async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
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
            var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
            // If products availabilities requested
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadInventories))
            {
                var inventories = new List<InventoryInfo>();

                var countResult = await _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria
                {
                    ProductIds = productIds,
                });

                var pageSize = 10;

                for (var i = 0; i < countResult.TotalCount; i += pageSize)
                {
                    var searchResult = await _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria
                    {
                        ProductIds = productIds,
                        Skip = i,
                        Take = pageSize,
                    });

                    inventories.AddRange(searchResult.Results);
                }

                if (inventories.Any())
                {
                    parameter.Results.Apply(x => x.ApplyStoreInventories(inventories, parameter.Store));
                }
            }

            await next(parameter);
        }
    }
}
