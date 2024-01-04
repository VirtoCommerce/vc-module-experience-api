using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveWishlistItemsCommand : WishlistCommand
    {
        public IList<string> LineItemIds { get; set; } = new List<string>();
    }
}
