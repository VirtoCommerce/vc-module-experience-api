using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCartItemsCommand : CartCommand
    {
        public RemoveCartItemsCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public RemoveCartItemsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string[] lineItemIds)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            LineItemIds = lineItemIds;
        }

        public string[] LineItemIds { get; set; }
    }
}
