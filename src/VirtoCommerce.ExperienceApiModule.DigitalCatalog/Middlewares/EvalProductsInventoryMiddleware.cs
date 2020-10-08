using System;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Services;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsInventoryMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IExpInventorySearchService _inventorySearchServiceExp;

        public EvalProductsInventoryMiddleware(IExpInventorySearchService inventorySearchServiceExp)
        {
            _inventorySearchServiceExp = inventorySearchServiceExp;
        }

        public virtual async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            parameter = await _inventorySearchServiceExp.SearchInventoriesAsync(parameter);

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


            await next(parameter);
        }


    }
}
