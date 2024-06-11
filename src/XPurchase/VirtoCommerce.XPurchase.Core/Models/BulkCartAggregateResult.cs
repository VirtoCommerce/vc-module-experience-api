using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Core.Models
{
    public class BulkCartAggregateResult
    {
        public IList<CartAggregate> CartAggregates { get; set; } = new List<CartAggregate>();
    }
}
