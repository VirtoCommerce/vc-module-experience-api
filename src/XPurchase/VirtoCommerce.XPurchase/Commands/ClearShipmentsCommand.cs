namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearShipmentsCommand : CartCommand
    {
        public ClearShipmentsCommand()
        {
        }

        public ClearShipmentsCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
        }
    }
}
