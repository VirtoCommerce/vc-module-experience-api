namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCartItemCommand : CartCommand
    {
        public RemoveCartItemCommand()
        {
        }

        public RemoveCartItemCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string productId)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            ProductId = productId;
        }

        public string ProductId { get; set; }
    }
}
