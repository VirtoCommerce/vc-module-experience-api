using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Queries
{
    public class SearchProductResponse 
    {
        public int TotalCount { get; set; }
        public IList<ExpProduct> Results { get; set; }
        public IList<FacetResult> Facets { get; set; }

    }
}
