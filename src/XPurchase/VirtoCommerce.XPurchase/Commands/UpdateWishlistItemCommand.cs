namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateWishlistItemCommand : WishlistCommand
    {
        public string LineItemId { get; set; }

        public int Quantity { get; set; }
    }
}
