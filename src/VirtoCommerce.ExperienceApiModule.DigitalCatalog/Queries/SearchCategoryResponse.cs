using System.Collections.Generic;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchCategoryResponse
    {
        public int TotalCount { get; set; }
        public IList<ExpCategory> Results { get; set; }
        public IList<FacetResult> Facets { get; set; }
    }
}
