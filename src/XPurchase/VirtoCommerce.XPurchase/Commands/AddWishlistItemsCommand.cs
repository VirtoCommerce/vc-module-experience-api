using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddWishlistItemsCommand : AddCartItemsBulkCommand
    {
        public string ListId { get => CartId; set => CartId = value; }

        public IList<NewBulkCartItem> ListItems { get => CartItems; set => CartItems = value; }
    }
}
