using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.Tools;
using VirtoCommerce.XDigitalCatalog.Breadcrumbs;
using toolsDto = VirtoCommerce.Tools.Models;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{

    public static class OutlineExtensions
    {
        /// <summary>
        /// Returns SEO path if all outline items of the first outline have SEO keywords, otherwise returns default value.
        /// Path: GrandParentCategory/ParentCategory/ProductCategory/Product
        /// </summary>
        /// <param name="outlines"></param>
        /// <param name="store"></param>
        /// <param name="language"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetSeoPath(this IEnumerable<Outline> outlines, Store store, string language, string defaultValue)
        {
            string result = null;
            EnumUtility.SafeParse(store.Settings.GetSettingValue("Stores.SeoLinksType", ""), toolsDto.SeoLinksType.Collapsed);

            var toolsStore = new toolsDto.Store
            {
                Id = store.Id,
                Url = store.Url,
                SecureUrl = store.SecureUrl,
                Catalog = store.Catalog,
                DefaultLanguage = store.DefaultLanguage,
                SeoLinksType = EnumUtility.SafeParse(store.Settings.GetSettingValue("Stores.SeoLinksType", ""), toolsDto.SeoLinksType.Collapsed),
                Languages = store.Languages?.ToList(),
            };
            var toolsOutlines = outlines?.Select(o => o.JsonConvert<toolsDto.Outline>()).ToArray();
            if (toolsOutlines != null)
            {
                result = toolsOutlines.GetSeoPath(toolsStore, language ?? store.DefaultLanguage, defaultValue);
            }
            return result;
        }

        /// <summary>
        /// Returns best matching outline path for the given catalog: CategoryId/CategoryId2.
        /// </summary>
        /// <param name="outlines"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public static string GetOutlinePath(this IEnumerable<Outline> outlines, string catalogId)
        {
            return outlines?.Select(o => o.JsonConvert<toolsDto.Outline>()).GetOutlinePath(catalogId);
        }

        /// <summary>
        /// Returns product's category outline.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static string GetCategoryOutline(this CatalogProduct product)
        {
            var result = string.Empty;

            if (product != null && !string.IsNullOrEmpty(product.Outline))
            {
                var i = product.Outline.LastIndexOf('/');
                if (i >= 0)
                {
                    result = product.Outline.Substring(0, i);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all concatinated relative outlines for the given catalog
        /// </summary>
        /// <param name="outlines"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public static string GetOutlinePaths(this IEnumerable<Outline> outlines, string catalogId)
        {
            var result = string.Empty;
            var catalogOutlines = outlines?.Where(o => o.Items.Any(i => i.SeoObjectType == "Catalog" && i.Id == catalogId));
            var outlinesList = catalogOutlines?.Where(x => x != null).Select(x => x.ToCatalogRelativePath()).ToList();

            if (!outlinesList.IsNullOrEmpty())
            {
                result = string.Join(";", outlinesList);
            }

            return result;
        }

        /// <summary>s
        /// Returns catalog's relative outline path
        /// </summary>
        /// <param name="outline"></param>
        /// <returns></returns>
        public static string ToCatalogRelativePath(this Outline outline)
        {
            return outline.Items == null ? null : string.Join("/",
                outline.Items
                    .Where(x => x != null && x.SeoObjectType != "Catalog")
                    .Select(x => x.Id)
                );
        }

        public static IEnumerable<Breadcrumb> GetBreadcrumbsFromOutLine(this IEnumerable<Outline> outlines, Store store, string cultureName)
        {

            var outlineFilteredByCatalog = outlines.FirstOrDefault(outline => outline.Items.Any(item => item.Id == store.Catalog && item.SeoObjectType == "Catalog"));
            var breadcrumbs = new List<Breadcrumb>();
            var outlineItems = outlineFilteredByCatalog?.Items.ToList();

            if (outlineItems.IsNullOrEmpty())
            {
                return breadcrumbs;
            }

            for (var i = outlineItems.Count - 1; i > 0; i--)
            {
                var item = outlineItems[i];

                var innerOutline = new List<Outline> { new Outline { Items = outlineItems } };
                var seoPath = innerOutline.GetSeoPath(store, cultureName, null);

                outlineItems.Remove(item);
                if (string.IsNullOrWhiteSpace(seoPath)) continue;

                var breadcrumb = new Breadcrumb(item.SeoObjectType)
                {
                    ItemId = item.Id,
                    Title = item.SeoInfos?.FirstOrDefault()?.SemanticUrl ?? item.Name,
                    SeoPath = seoPath
                };
                breadcrumbs.Insert(0, breadcrumb);
            }
            return breadcrumbs;

        }
    }
}
