using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartShipmentCommand : CartCommand
    {
        public AddOrUpdateCartShipmentCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public AddOrUpdateCartShipmentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, ExpCartShipment shipment)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Shipment = shipment;
        }

        public ExpCartShipment Shipment { get; set; }
    }
}
