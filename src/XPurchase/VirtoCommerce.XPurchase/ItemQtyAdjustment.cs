namespace VirtoCommerce.XPurchase
{
    public class ItemQtyAdjustment
    {
        public string LineItemId { get; set; }
        public int NewQuantity { get; set; }
        public CartProduct CartProduct { get; set; }
    }
}
