using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.XPurchase.Services
{
    public interface ICartResponseGroupParser
    {
        string GetResponseGroup(IList<string> includeFields);
    }

    public class CartResponseGroupParser : ICartResponseGroupParser
    {
        public virtual string GetResponseGroup(IList<string> includeFields)
        {
            var result = CartAggregateResponseGroup.Full;

            // Disable recalculation of totals, XAPI will do it on its own
            result &= ~CartAggregateResponseGroup.RecalculateTotals;

            if (!includeFields.Any(x => x.Contains("dynamicProperties")))
            {
                // Disable load of dynamic properties if not requested
                result &= ~CartAggregateResponseGroup.WithDynamicProperties;
            }

            return result.ToString();
        }
    }
}
