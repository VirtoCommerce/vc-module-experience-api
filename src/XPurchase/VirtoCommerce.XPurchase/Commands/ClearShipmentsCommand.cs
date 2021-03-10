namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearShipmentsCommand : CartCommand
    {
        public ClearShipmentsCommand()
        {
        }

        public ClearShipmentsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
        }
    }
}
