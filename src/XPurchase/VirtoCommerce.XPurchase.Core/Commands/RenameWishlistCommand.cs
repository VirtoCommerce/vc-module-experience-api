using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class RenameWishlistCommand : WishlistCommand
    {
        public string ListName { get; set; }

        public RenameWishlistCommand(string listId, string listName)
        {
            ListId = listId;
            ListName = listName;
        }
    }
}
