using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddOrUpdateCartShipmentType : InputCartBaseType
    {
        public InputAddOrUpdateCartShipmentType()
        {
            Field<NonNullGraphType<InputShipmentType>>("shipment");
        }
    }

    public class InputClearPaymentsType : InputCartBaseType { }
}
