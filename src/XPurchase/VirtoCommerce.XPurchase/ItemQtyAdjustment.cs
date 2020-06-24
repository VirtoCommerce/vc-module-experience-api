namespace VirtoCommerce.XPurchase.Domain.CartAggregate
{
    public class ItemQtyAdjustment
    {
        public ItemQtyAdjustment(string lineItemId, int newQty)
        {
            LineItemId = lineItemId;
            NewQuantity = newQty;

        }
        public string LineItemId { get; private set; }
        public int NewQuantity { get; private set; }
    }
}
