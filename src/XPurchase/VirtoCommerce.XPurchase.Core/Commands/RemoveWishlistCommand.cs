using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class RemoveWishlistCommand : WishlistCommand
    {
        public RemoveWishlistCommand(string listId)
        {
            ListId = listId;
        }
    }
}
