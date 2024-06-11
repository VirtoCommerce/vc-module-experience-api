using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;

namespace VirtoCommerce.XOrder.Core.Queries
{
    public class SearchOrderResponse
    {
        public int TotalCount { get; set; }
        public IList<CustomerOrderAggregate> Results { get; set; }
        public IList<FacetResult> Facets { get; set; }
    }
}
