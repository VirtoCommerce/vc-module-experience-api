using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputAddOrUpdateCartShipmentType : InputCartBaseType
    {
        public InputAddOrUpdateCartShipmentType()
        {
            Field<NonNullGraphType<InputShipmentType>>("shipment",
                "Shipment");
        }
    }
}
