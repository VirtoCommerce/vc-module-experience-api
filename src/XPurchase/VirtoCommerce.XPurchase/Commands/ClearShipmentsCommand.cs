using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearShipmentsCommand : CartCommand
    {
        public ClearShipmentsCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public ClearShipmentsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
        }
    }
}
