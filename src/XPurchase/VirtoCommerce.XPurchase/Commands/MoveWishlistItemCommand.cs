namespace VirtoCommerce.XPurchase.Commands
{
    public class MoveWishlistItemCommand : WishlistCommand
    {
        public string DestinationListId { get; set; }

        public string LineItemId { get; set; }

        public MoveWishlistItemCommand(string listId, string destinationListId, string lineItemId)
        {
            ListId = listId;
            DestinationListId = destinationListId;
            LineItemId = lineItemId;
        }
    }
}
