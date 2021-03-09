using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Breadcrumbs;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class ProductExtension
    {
        public static IEnumerable<Breadcrumb> GetBreadcrumbs(this CatalogProduct product, Store store, string cultureName)
        {
            if (product.Category == null)
            {
                yield return new ProductBreadcrumb(product)
                {
                    Title = product.Name,
                    SeoPath = product.Outlines.GetSeoPath(store, cultureName, null)
                };
            }
            else
            {
                foreach (var breadCrumb in product.Category.GetBreadcrumbs(store, cultureName).Distinct())
                {
                    yield return breadCrumb;
                }
            }
        }
    }
}
