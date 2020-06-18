using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.XPurchase.Models.Extensions;

namespace VirtoCommerce.XPurchase.Models.Catalog
{
    public static class AggregationExtension
    {
        public static IEnumerable<AggregationItem> FindItemsByFieldName(this IEnumerable<Aggregation> aggregations, string fieldName)
        {
            if (aggregations == null)
            {
                throw new ArgumentNullException(nameof(aggregations));
            }
            return aggregations.Where(x => x.Field.EqualsInvariant(fieldName))
                               .SelectMany(x => x.Items)
                               .Where(x => x.Value != null);
        }
    }
}
