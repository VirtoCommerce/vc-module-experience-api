using System;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.Platform.Core.Common;
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

            var query = parameter.Query;
            if (query == null)
            {
                throw new OperationCanceledException("Query must be set");
            }

            var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
            // If products availabilities requested
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadInventories))
            {
                parameter = await _inventorySearchServiceExp.SearchInventoriesAsync(parameter);
            }
                       
            await next(parameter);
        }


    }
}
