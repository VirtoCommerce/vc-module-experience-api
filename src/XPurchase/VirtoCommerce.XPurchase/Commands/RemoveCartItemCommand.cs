namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCartItemCommand : CartCommand
    {
        public RemoveCartItemCommand()
        {
        }

        public RemoveCartItemCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string lineItemId)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            LineItemId = lineItemId;
        }

        public string LineItemId { get; set; }
    }
}
