using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Core.Services
{
    public interface ICartResponseGroupParser
    {
        string GetResponseGroup(IList<string> includeFields);
    }
}
