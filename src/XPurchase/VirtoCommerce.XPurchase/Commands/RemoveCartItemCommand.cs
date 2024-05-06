namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCartItemCommand : CartCommand
    {
        public RemoveCartItemCommand()
        {
        }

        public RemoveCartItemCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string lineItemId)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            LineItemId = lineItemId;
        }

        public string LineItemId { get; set; }
    }
}
