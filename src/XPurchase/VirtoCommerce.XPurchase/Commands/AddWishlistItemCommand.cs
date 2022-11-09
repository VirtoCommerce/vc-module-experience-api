namespace VirtoCommerce.XPurchase.Commands
{
    public class AddWishlistItemCommand : WishlistCommand
    {
        public string ProductId { get; set; }
        public int? Quantity { get; set; }

        public AddWishlistItemCommand(string listId, string productId)
        {
            ListId = listId;
            ProductId = productId;
        }
    }
}
