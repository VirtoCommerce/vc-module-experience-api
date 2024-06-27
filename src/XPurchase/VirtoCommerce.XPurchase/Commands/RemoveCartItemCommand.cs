using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCartItemCommand : CartCommand
    {
        public RemoveCartItemCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public RemoveCartItemCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string lineItemId)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            LineItemId = lineItemId;
        }

        public string LineItemId { get; set; }
    }
}
