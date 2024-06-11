using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class AddOrUpdateCartShipmentCommand : CartCommand
    {
        public AddOrUpdateCartShipmentCommand()
        {
        }

        public AddOrUpdateCartShipmentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, ExpCartShipment shipment)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Shipment = shipment;
        }

        public ExpCartShipment Shipment { get; set; }
    }
}
