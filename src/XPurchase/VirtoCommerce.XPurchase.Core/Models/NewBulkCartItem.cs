namespace VirtoCommerce.XPurchase.Core.Models
{
    /// <summary>
    /// Used in cart bulk mutations
    /// </summary>
    public class NewBulkCartItem
    {
        public NewBulkCartItem(string productSku, int quantity)
        {
            ProductSku = productSku;
            Quantity = quantity;
        }

        public string ProductSku { get; private set; }

        public int Quantity { get; private set; }
    }
}
