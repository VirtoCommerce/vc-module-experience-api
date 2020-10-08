using System.Linq;

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
            return result.ToString();
        }

    }
}
