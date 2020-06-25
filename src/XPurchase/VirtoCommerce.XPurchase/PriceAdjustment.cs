using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase
{
    public class PriceAdjustment : ValueObject
    {
        public PriceAdjustment(string lineItemId, decimal newPrice)
        {
            LineItemId = lineItemId;
            NewPrice = newPrice;
        }
        public string LineItemId { get; private set; }
        public decimal NewPrice { get; private set; }
    }
}
