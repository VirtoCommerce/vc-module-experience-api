namespace VirtoCommerce.XPurchase
{
    public class ItemQtyAdjustment
    {
        public ItemQtyAdjustment(string lineItemId, int newQty, CartProduct cartProduct)
        {
            LineItemId = lineItemId;
            NewQuantity = newQty;
            CartProduct = cartProduct;

        }
        public string LineItemId { get; private set; }
        public int NewQuantity { get; private set; }
        public CartProduct CartProduct { get; private set; }
    }
}
