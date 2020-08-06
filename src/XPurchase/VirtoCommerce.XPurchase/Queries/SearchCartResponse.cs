using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Queries
{
    public class SearchCartResponse
    {
        public int TotalCount { get; set; }
        public IList<CartAggregate> Results { get; set; }
    }
}
