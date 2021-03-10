using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartShipmentCommand : CartCommand
    {
        public AddOrUpdateCartShipmentCommand()
        {
        }

        public AddOrUpdateCartShipmentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, Shipment shipment)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Shipment = shipment;
        }

        public Shipment Shipment { get; set; }
    }
}
