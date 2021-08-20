using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class RemoveNullCatalogProductsMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IItemService _itemService;

        public RemoveNullCatalogProductsMiddleware(IItemService itemService)
        {
            _itemService = itemService;
        }

        public Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            var missingItems = parameter.Results
                .Where(expProduct => expProduct.IndexedProduct is null)
                .ToArray();

            foreach (var missingItem in missingItems)
            {
                parameter.Results.Remove(missingItem);
            }

            parameter.TotalCount -= missingItems.Length;

            return next(parameter);
        }
    }
}
