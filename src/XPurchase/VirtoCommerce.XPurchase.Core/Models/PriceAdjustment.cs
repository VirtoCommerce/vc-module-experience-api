using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Core.Models
{
    public class PriceAdjustment : ValueObject
    {
        public LineItem LineItem { get; set; }
        public string LineItemId { get; set; }
        public decimal NewPrice { get; set; }
    }
}
