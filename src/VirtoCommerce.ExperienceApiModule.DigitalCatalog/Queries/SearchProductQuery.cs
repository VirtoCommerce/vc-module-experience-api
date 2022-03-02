using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductQuery : CatalogQueryBase<SearchProductResponse>
    {
        public string Query { get; set; }
        public bool Fuzzy { get; set; }
        public int? FuzzyLevel { get; set; }
        public string Filter { get; set; }
        public string Facet { get; set; }
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string[] ObjectIds { get; set; }

        public virtual string GetResponseGroup()
        {
            var result = ExpProductResponseGroup.None;
            if (IncludeFields.Any(x => x.Contains("price")))
            {
                result |= ExpProductResponseGroup.LoadPrices;
            }
            if (IncludeFields.Any(x => x.Contains("availabilityData")))
            {
                result |= ExpProductResponseGroup.LoadInventories;
            }
            if (IncludeFields.Any(x => x.Contains("_facets")))
            {
                result |= ExpProductResponseGroup.LoadFacets;
            }
            return result.ToString();
        }

        public virtual string GetItemResponseGroup()
        {
            var result = ItemResponseGroup.None;

            if (IncludeFields.ContainsAny("assets", "images", "imgSrc"))
            {
                result |= ItemResponseGroup.WithImages;
            }

            if (IncludeFields.ContainsAny("properties", "brandName"))
            {
                result |= ItemResponseGroup.WithProperties;
            }

            // "descriptions" could look redundant, but better to check it explicitly - clear approach for possible modification or different "Contains" logic
            if (IncludeFields.ContainsAny("description", "descriptions"))
            {
                result |= ItemResponseGroup.ItemEditorialReviews;
            }

            if (IncludeFields.ContainsAny("seoInfo"))
            {
                result |= ItemResponseGroup.WithSeo;
            }

            if (IncludeFields.ContainsAny("slug"))
            {
                result |= ItemResponseGroup.WithLinks;
                result |= ItemResponseGroup.WithSeo;
            }

            if (IncludeFields.ContainsAny("outline", "outlines", "slug", "level", "breadcrumbs"))
            {
                result |= ItemResponseGroup.WithOutlines;
                result |= ItemResponseGroup.WithSeo;
            }

            if (IncludeFields.ContainsAny("availabilityData"))
            {
                result |= ItemResponseGroup.Inventory;
            }

            return result.ToString();
        }
    }
}
