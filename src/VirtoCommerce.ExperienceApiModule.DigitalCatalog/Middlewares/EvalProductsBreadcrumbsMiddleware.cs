using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsBreadcrumbsMiddleware: IAsyncMiddleware<SearchProductResponse>
    {
        private readonly ICategorySearchService _searchService;
        private readonly IStoreService _storeService;

        public EvalProductsBreadcrumbsMiddleware(ICategorySearchService searchService, IStoreService storeService)
        {
            _searchService = searchService;
            _storeService = storeService;
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
                var categoryIds = parameter.Results.Select(x => x.IndexedProduct.CategoryId).Distinct().ToArray();

                var store = await _storeService.GetByIdAsync(query.StoreId, StoreResponseGroup.StoreInfo.ToString());
                var categories = await _searchService.SearchCategoriesAsync(new CategorySearchCriteria() { CatalogId = store.Catalog, ObjectIds = categoryIds });

                foreach (var product in parameter.Results.Where(x=>x.IndexedProduct != null))
                {
                    var outlineItems = product.IndexedProduct.Outlines.SelectMany(x=>x.Items.Select(d=>d.Id));
                    var category = categories.Results.FirstOrDefault(x => outlineItems.Contains(x.Id));
                    product.IndexedProduct.Category = category;
                }
            }

            await next(parameter);
        }

        private string JoinResponseGroups(params CategoryResponseGroup[] responseGroups) => string.Join(",", responseGroups);
    }
}
