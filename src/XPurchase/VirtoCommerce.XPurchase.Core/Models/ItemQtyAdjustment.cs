using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Core.Models
{
    public class ItemQtyAdjustment
    {
        public LineItem LineItem { get; set; }
        public string LineItemId { get; set; }
        public int NewQuantity { get; set; }
        public CartProduct CartProduct { get; set; }
    }
}
