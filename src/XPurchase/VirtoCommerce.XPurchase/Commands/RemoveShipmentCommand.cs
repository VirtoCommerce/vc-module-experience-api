using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveShipmentCommand : CartCommand
    {
        public RemoveShipmentCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public RemoveShipmentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string shipmentId)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            ShipmentId = shipmentId;
        }

        public string ShipmentId { get; set; }
    }
}
