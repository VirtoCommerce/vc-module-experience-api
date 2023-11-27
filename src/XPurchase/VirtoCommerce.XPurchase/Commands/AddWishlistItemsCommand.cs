using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddWishlistItemsCommand : WishlistCommand
    {
        public IList<NewCartItem> ListItems { get; set; } = new List<NewCartItem>();
    }
}
