namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemQuantityCommand : CartCommand
    {
        public ChangeCartItemQuantityCommand()
        {
        }

        public ChangeCartItemQuantityCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string lineItemId, int quantity)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            LineItemId = lineItemId;
            Quantity = quantity;
        }

        public string LineItemId { get; set; }
        public int Quantity { get; set; }
    }
}
