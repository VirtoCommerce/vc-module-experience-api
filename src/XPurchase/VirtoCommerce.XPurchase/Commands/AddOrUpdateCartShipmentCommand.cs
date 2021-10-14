using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartShipmentCommand : CartCommand
    {
        public AddOrUpdateCartShipmentCommand()
        {
        }

        public AddOrUpdateCartShipmentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, ShipmentOptional shipment)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Shipment = shipment;
        }

        public ShipmentOptional Shipment { get; set; }
    }
}
