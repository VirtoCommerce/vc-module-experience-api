using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemPriceCommand : CartCommand
    {
        public ChangeCartItemPriceCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public ChangeCartItemPriceCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string lineItemId, decimal price)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            LineItemId = lineItemId;
            Price = price;
        }

        public string LineItemId { get; set; }

        /// <summary>
        /// Manual price
        /// </summary>
        public decimal Price { get; set; }
    }
}
