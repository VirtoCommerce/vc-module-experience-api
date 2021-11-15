using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
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
