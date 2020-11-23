using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.XDigitalCatalog.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchCategoryQuery : CatalogQueryBase<SearchCategoryResponse>
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

        public virtual string GetCategoryResponseGroup()
        {
            var result = CategoryResponseGroup.None;

            if (IncludeFields.ContainsAny("imgSrc"))
            {
                result |= CategoryResponseGroup.WithImages;
            }

            if (IncludeFields.ContainsAny("properties"))
            {
                result |= CategoryResponseGroup.WithProperties;
            }

            if (IncludeFields.ContainsAny("seoInfo"))
            {
                result |= CategoryResponseGroup.WithSeo;
            }

            if (IncludeFields.ContainsAny("slug"))
            {
                result |= CategoryResponseGroup.WithLinks;
            }

            if (IncludeFields.ContainsAny("outline", "outlines"))
            {
                result |= CategoryResponseGroup.WithOutlines;
            }

            return result.ToString();
        }
    }
}
