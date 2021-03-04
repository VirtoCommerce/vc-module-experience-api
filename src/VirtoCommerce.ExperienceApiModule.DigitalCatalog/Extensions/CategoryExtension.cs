using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Breadcrumbs;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class CategoryExtension
    {
        public static IEnumerable<Breadcrumb> GetBreadcrumbs(this Category category, Store store, string cultureName)
        {
            if (category.Outlines == null)
            {
                throw new ArgumentException("Category outlines can't be null");
            }

            var seoPath = category.Outlines.GetSeoPath(store, cultureName, null);
            foreach (var parentCategory in category.Parents?.Distinct() ?? new List<Category>())
            {
                var parentSeoPath = parentCategory.Outlines.GetSeoPath(store, cultureName, null);
                if (!parentSeoPath.IsNullOrEmpty())
                {
                    yield return new CategoryBreadcrumb(parentCategory)
                    {
                        SeoPath = parentSeoPath,
                        Url = parentSeoPath,
                        Title = parentCategory.Name
                    };
                }
            }

            if (!seoPath.IsNullOrEmpty())
            {
                yield return new CategoryBreadcrumb(category)
                {
                    Title = category.Name,
                    SeoPath = seoPath
                };
            }
        }
    }
}
