using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    /// <summary>
    /// This middleware enriches outlines with names.
    /// Just reads names of catalogs and categories in outlines thru ICatalogService, ICategoryService
    /// </summary>
    public class EnsureCategoryOutlineNamesLoadedMiddleware : IAsyncMiddleware<SearchCategoryResponse>
    {
        private readonly ICatalogService _catalogService;
        private readonly ICategoryService _categoryService;

        public EnsureCategoryOutlineNamesLoadedMiddleware(ICatalogService catalogService, ICategoryService categoryService)
        {
            _catalogService = catalogService;
            _categoryService = categoryService;
        }

        public async Task Run(SearchCategoryResponse parameter, Func<SearchCategoryResponse, Task> next)
        {
            if (parameter.Query.IncludeFields.Contains("outlines.name"))
            {
                var outlineItems = parameter.Results.SelectMany(x => x.Category.Outlines).SelectMany(x => x.Items).Where(x => x.Name is null);
                var catalogOutlineItems = outlineItems.Where(x => x.SeoObjectType == "Catalog");
                var categoryOutlineItems = outlineItems.Where(x => x.SeoObjectType == "Category");
                var catalogs = (await _catalogService.GetByIdsAsync(catalogOutlineItems.Select(x => x.Id).Distinct().ToArray(), CatalogResponseGroup.Info.ToString())).ToDictionary(x => x.Id);
                var categories = (await _categoryService.GetByIdsAsync(categoryOutlineItems.Select(x => x.Id).Distinct().ToArray(), CategoryResponseGroup.Info.ToString())).ToDictionary(x => x.Id);

                foreach (var catalogOutlineItem in catalogOutlineItems)
                {
                    catalogOutlineItem.Name = catalogs.GetValueOrDefault(catalogOutlineItem.Id)?.Name;
                }

                foreach (var categoryOutlineItem in categoryOutlineItems)
                {
                    categoryOutlineItem.Name = categories.GetValueOrDefault(categoryOutlineItem.Id)?.Name;
                }
            }
            await next(parameter);
        }
    }
}
