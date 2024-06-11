using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class ChangeWishlistCommand : WishlistCommand
    {
        public string ListName { get; set; }

        public string Scope { get; set; }

        public string Description { get; set; }
    }
}
