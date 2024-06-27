using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearCartCommand : CartCommand
    {
        public ClearCartCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public ClearCartCommand(string storeId, string type, string cartName, string userId, string currencyCode, string cultureName)
            : base(storeId, type, cartName, userId, currencyCode, cultureName)
        {
        }
    }
}
