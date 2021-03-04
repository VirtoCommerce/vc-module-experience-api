namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearPaymentsCommand : CartCommand
    {
        public ClearPaymentsCommand()
        {
        }

        public ClearPaymentsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
        }
    }
}
