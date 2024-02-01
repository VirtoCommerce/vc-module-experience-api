using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
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
        private readonly IGenericPipelineLauncher _pipeline;

        public EvalProductsInventoryMiddleware(IInventorySearchService inventorySearchService, IGenericPipelineLauncher pipeline)
        {
            _inventorySearchService = inventorySearchService;
            _pipeline = pipeline;
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
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadInventories) &&
                productIds.Any())
            {
                var inventories = new List<InventoryInfo>();

                var skip = 0;
                var take = 50;
                InventoryInfoSearchResult searchResult;

                do
                {
                    var searchCriteria = await GetInventorySearchCriteria(productIds);

                    searchCriteria.Take = take;
                    searchCriteria.Skip = skip;

                    searchResult = await _inventorySearchService.SearchInventoriesAsync(searchCriteria);

                    inventories.AddRange(searchResult.Results);
                    skip += take;
                }
                while (searchResult.Results.Count == take);

                if (inventories.Any())
                {
                    parameter.Results.Apply(x => x.ApplyStoreInventories(inventories, parameter.Store));
                }
            }

            await next(parameter);
        }

        protected virtual async Task<InventorySearchCriteria> GetInventorySearchCriteria(IList<string> productIds)
        {
            var searchCreteria = AbstractTypeFactory<InventorySearchCriteria>.TryCreateInstance();

            searchCreteria.ProductIds = productIds;

            await _pipeline.Execute(searchCreteria);

            return searchCreteria;
        }
    }
}
