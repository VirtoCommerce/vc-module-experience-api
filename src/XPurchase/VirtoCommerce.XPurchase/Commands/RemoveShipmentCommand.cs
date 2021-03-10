namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveShipmentCommand : CartCommand
    {
        public RemoveShipmentCommand()
        {
        }

        public RemoveShipmentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string shipmentId)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            ShipmentId = shipmentId;
        }

        public string ShipmentId { get; set; }
    }
}
