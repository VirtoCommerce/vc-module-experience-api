using System.Collections.Generic;

namespace VirtoCommerce.XPurchase
{
    public class BulkCartAggregateResult
    {
        public IList<CartAggregate> CartAggregates { get; set; } = new List<CartAggregate>();
    }
}
