using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.XPurchase.Core.Models;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Services
{
    public class CartResponseGroupParser : ICartResponseGroupParser
    {
        public virtual string GetResponseGroup(IList<string> includeFields)
        {
            var result = CartAggregateResponseGroup.None;
            if (includeFields.Any(x => x.Contains("shipments")))
            {
                result |= CartAggregateResponseGroup.WithShipments;
            }
            if (includeFields.Any(x => x.Contains("payments")))
            {
                result |= CartAggregateResponseGroup.WithPayments;
            }
            if (includeFields.Any(x => x.Contains("items")))
            {
                result |= CartAggregateResponseGroup.WithLineItems;
            }
            if (includeFields.Any(x => x.Contains("dynamicProperties")))
            {
                result |= CartAggregateResponseGroup.WithDynamicProperties;
            }
            if (includeFields.Any(x => x.Contains("validationErrors")))
            {
                result |= CartAggregateResponseGroup.Validate;
            }

            return result.ToString();
        }
    }
}
