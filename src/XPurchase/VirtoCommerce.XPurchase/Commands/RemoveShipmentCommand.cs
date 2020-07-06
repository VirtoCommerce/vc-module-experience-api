namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveShipmentCommand : CartCommand
    {
        public RemoveShipmentCommand()
        {
        }

        public RemoveShipmentCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string shipmentId)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            ShipmentId = shipmentId;
        }

        public string ShipmentId { get; set; }
    }
}
