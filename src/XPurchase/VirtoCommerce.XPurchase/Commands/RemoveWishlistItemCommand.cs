namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveWishlistItemCommand : WishlistCommand
    {
        public string LineItemId { get; set; }

        public RemoveWishlistItemCommand(string listId, string lineItemId)
        {
            ListId = listId;
            LineItemId = lineItemId;
        }
    }
}
