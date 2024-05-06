using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EnsureCatalogProductLoadedMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IItemService _itemService;

        public EnsureCatalogProductLoadedMiddleware(IItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            var itemsIds = parameter.Results
                .Where(expProduct => expProduct.IndexedProduct is null)
                .Select(expProduct => expProduct.Key)
                .Where(key => key != null)
                .ToArray();

            if (itemsIds.Any())
            {
                var responseGroup = parameter.Query.GetItemResponseGroup();
                var catalogProducts = await _itemService.GetAsync(itemsIds, responseGroup);

                foreach (var catalogProduct in catalogProducts)
                {
                    var item = parameter.Results.FirstOrDefault(expProduct => expProduct.Key == catalogProduct.Id);
                    if (item is null)
                    {
                        continue;
                    }

                    item.IndexedProduct ??= catalogProduct;
                }
            }

            await next(parameter);
        }
    }
}
