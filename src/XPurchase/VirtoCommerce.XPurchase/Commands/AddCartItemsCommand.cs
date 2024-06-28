using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartItemsCommand : CartCommand
    {
        public AddCartItemsCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public AddCartItemsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, NewCartItem[] cartItems)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            CartItems = cartItems;
        }

        public NewCartItem[] CartItems { get; set; }
    }
}
