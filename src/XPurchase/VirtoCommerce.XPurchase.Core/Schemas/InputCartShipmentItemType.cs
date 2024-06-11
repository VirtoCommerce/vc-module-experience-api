using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputCartShipmentItemType : InputObjectGraphType
    {
        public InputCartShipmentItemType()
        {
            Field<NonNullGraphType<IntGraphType>>("quantity",
                "Quantity");
            Field<NonNullGraphType<StringGraphType>>("lineItemId",
                "Line item ID");
        }
    }
}
