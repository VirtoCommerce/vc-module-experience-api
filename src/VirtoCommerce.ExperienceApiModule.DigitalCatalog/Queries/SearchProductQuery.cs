using System.Collections.Generic;

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
        public IEnumerable<string> ProductIds { get; set; }
    }
}
