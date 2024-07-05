using System;

namespace VirtoCommerce.XPurchase.Commands
{
    /// <summary>
    /// Designed to refresh the contents of a shopping cart without making any modifications to the items in it. Its primary purpose is to update the cart's price and to hide any warnings or errors that may have occurred since the cart was last loaded.
    /// </summary>
    public class RefreshCartCommand : CartCommand
    {
        public RefreshCartCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public RefreshCartCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
        }
    }
}
