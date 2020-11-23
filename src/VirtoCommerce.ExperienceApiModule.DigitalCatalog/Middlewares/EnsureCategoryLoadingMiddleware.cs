using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EnsureCategoryLoadingMiddleware : IAsyncMiddleware<SearchCategoryResponse>
    {
        private readonly ICategoryService _categoryService;

        public EnsureCategoryLoadingMiddleware(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task Run(SearchCategoryResponse parameter, Func<SearchCategoryResponse, Task> next)
        {
            var itemsIds = parameter.Results
                .Where(expProduct => expProduct.Category is null)
                .Select(expProduct => expProduct.Key)
                .Where(key => key != null)
                .ToArray();

            if (itemsIds.Any())
            {
                string responceGroup = null/*parameter.Query.GetResponseGroup()*/; // todo: responce group for category
                var categories = await _categoryService.GetByIdsAsync(itemsIds, responceGroup);

                foreach (var category in categories)
                {
                    var item = parameter.Results.FirstOrDefault(expProduct => expProduct.Key == category.Id);
                    if (item is null) continue;
                    item.Category ??= category;
                }
            }

            await next(parameter);
        }
    }
}
