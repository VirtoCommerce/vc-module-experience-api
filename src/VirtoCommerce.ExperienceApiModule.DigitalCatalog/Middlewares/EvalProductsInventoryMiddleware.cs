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

        public async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            parameter = await _inventorySearchServiceExp.SearchInventoriesAsync(parameter);

            await next(parameter);
        }


    }
}
