using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartShipmentCommand : CartCommand
    {
        public AddOrUpdateCartShipmentCommand()
        {
        }

        public AddOrUpdateCartShipmentCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, Shipment shipment)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            Shipment = shipment;
        }

        public Shipment Shipment { get; set; }
    }
}
