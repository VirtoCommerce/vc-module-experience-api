using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Commands.BaseCommands
{
    public abstract class WishlistCommand : CartCommand
    {
        public string ListId { get; set; }

        public WishlistUserContext WishlistUserContext { get; set; }
    }
}
