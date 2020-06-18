using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Models.Common
{
    public interface IHasQueryKeyValues
    {
        IEnumerable<KeyValuePair<string, string>> GetQueryKeyValues();
    }
}
