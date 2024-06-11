using System.Collections.Generic;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class AddWishlistItemsCommand : WishlistCommand
    {
        public IList<NewCartItem> ListItems { get; set; } = new List<NewCartItem>();
    }
}
