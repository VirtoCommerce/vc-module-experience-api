namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemQuantityCommand : CartCommand
    {
        public ChangeCartItemQuantityCommand()
        {
        }

        public ChangeCartItemQuantityCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string productId, int quantity)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
