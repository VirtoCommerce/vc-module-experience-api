using System.Collections.Generic;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class UpdateWishlistItemsCommand : WishlistCommand
    {
        public List<WishListItem> Items { get; set; } = new List<WishListItem>();
    }
}
