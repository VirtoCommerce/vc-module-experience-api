using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsBreadcrumbsMiddleware: IAsyncMiddleware<SearchProductResponse>
    {
        private readonly ICategoryService _categoryService;

        public EvalProductsBreadcrumbsMiddleware(ICategoryService categoryService)
        {
            _categoryService = categoryService;
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

            if (query.IncludeFields.ContainsAny("breadcrumbs"))
            {
                var categoryIds = parameter.Results.Select(x => x.IndexedProduct.CategoryId).ToArray();
                var categories = await _categoryService.GetByIdsAsync(categoryIds, string.Join(",", CategoryResponseGroup.WithParents.ToString(), CategoryResponseGroup.WithOutlines.ToString()));

                foreach (var product in parameter.Results)
                {
                    var category = categories.FirstOrDefault(x => x.Id == product.IndexedProduct.CategoryId);
                    product.IndexedProduct.Category = category;
                }

            }

            await next(parameter);
        }
    }
}
