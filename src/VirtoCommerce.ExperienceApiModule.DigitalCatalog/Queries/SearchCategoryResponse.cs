using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchCategoryResponse
    {
        public int TotalCount { get; set; }
        public IList<ExpCategory> Results { get; set; }
        public IList<FacetResult> Facets { get; set; }
    }
}
