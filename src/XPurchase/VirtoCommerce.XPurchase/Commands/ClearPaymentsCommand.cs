namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearPaymentsCommand : CartCommand
    {
        public ClearPaymentsCommand()
        {
        }

        public ClearPaymentsCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
        }
    }
}
