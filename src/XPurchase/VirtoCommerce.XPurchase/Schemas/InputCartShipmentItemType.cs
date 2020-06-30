using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputCartShipmentItemType : InputObjectGraphType
    {
        public InputCartShipmentItemType()
        {
            Field<NonNullGraphType<IntGraphType>>("quantity");
            Field<NonNullGraphType<StringGraphType>>("lineItemId");
        }
    }
}
