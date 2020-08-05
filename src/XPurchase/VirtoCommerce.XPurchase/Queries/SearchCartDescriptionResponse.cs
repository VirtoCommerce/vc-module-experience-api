using System.Collections.Generic;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Queries
{
    public class SearchCartDescriptionResponse
    {
        public int TotalCount { get; set; }
        public IList<CartDescription> Results { get; set; }
    }
}
