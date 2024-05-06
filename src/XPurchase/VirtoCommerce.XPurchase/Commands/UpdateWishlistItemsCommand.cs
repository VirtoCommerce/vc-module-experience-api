using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateWishlistItemsCommand : WishlistCommand
    {
        public List<WishListItem> Items { get; set; } = new List<WishListItem>();
    }
}
