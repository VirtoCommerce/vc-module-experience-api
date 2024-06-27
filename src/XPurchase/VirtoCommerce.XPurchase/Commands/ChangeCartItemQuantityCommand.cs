using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemQuantityCommand : CartCommand
    {
        public ChangeCartItemQuantityCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public ChangeCartItemQuantityCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string lineItemId, int quantity)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            LineItemId = lineItemId;
            Quantity = quantity;
        }

        public string LineItemId { get; set; }
        public int Quantity { get; set; }
    }
}
