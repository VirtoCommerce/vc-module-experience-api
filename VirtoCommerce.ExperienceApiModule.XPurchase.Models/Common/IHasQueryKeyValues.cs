using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public interface IHasQueryKeyValues
    {
        IEnumerable<KeyValuePair<string, string>> GetQueryKeyValues();
    }
}
