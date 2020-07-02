using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Index;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Requests
{
    public class SearchProductResponse 
    {
        public int TotalCount { get; set; }
        public IList<ExpProduct> Results { get; set; }
        public IList<FacetResult> Facets { get; set; }

    }
}
