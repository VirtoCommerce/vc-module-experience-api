using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputRemoveShipmentType : InputCartBaseType
    {
        public InputRemoveShipmentType()
        {
            Field<StringGraphType>("shipmentId",
                "Shipment Id");
        }
    }
}
