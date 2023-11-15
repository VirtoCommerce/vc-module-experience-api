using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeWishlistCommand : WishlistCommand
    {
        public string ListName { get; set; }

        public string Scope { get; set; }

        public string Description { get; set; }

        public Contact Contact { get; set; }

        public ShoppingCart Cart { get; set; }
    }
}
