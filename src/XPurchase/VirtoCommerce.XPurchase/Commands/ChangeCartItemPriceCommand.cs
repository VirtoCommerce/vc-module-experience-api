namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemPriceCommand : CartCommand
    {
        public ChangeCartItemPriceCommand()
        {
        }

        public ChangeCartItemPriceCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string lineItemId, decimal price)
            : base(storeId, cartType, cartName, userId, currency, lang)
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
