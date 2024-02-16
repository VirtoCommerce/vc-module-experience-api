namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveWishlistCommand : WishlistCommand
    {
        public RemoveWishlistCommand(string listId)
        {
            ListId = listId;
        }
    }
}
