using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class LoadProductsInventoriesFromSomewhereMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        public async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            foreach(var product in parameter.Results)
            {
                var inventoriesFromSomewhere = new[] { new InventoryModule.Core.Model.InventoryInfo { ProductId = product.Id, InStockQuantity = 100, FulfillmentCenterId = parameter.Store.MainFulfillmentCenterId } };
                product.ApplyStoreInventories(inventoriesFromSomewhere, parameter.Store);
            }

            await next(parameter);
        }
    }
}
