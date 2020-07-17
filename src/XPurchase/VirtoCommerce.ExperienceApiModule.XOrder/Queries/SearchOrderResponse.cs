using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrderResponse
    {
        public int TotalCount { get; set; }
        public IList<CustomerOrderAggregate> Results { get; set; }
    }
}
