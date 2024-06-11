using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.XPurchase.Core.Models
{
    public class WishlistUserContext
    {
        public string CurrentUserId { get; set; }

        public Contact CurrentContact { get; set; }

        public ShoppingCart Cart { get; set; }

        public string UserId { get; set; }

        public string Scope { get; set; }
    }
}
