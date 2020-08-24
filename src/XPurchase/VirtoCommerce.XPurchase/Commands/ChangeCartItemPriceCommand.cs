namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemPriceCommand : CartCommand
    {
        public ChangeCartItemPriceCommand()
        {
        }

        public ChangeCartItemPriceCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string productId, decimal price)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            ProductId = productId;
            Price = price;
        }

        public string ProductId { get; set; }

        /// <summary>
        /// Manual price
        /// </summary>
        public decimal Price { get; set; }
    }
}
