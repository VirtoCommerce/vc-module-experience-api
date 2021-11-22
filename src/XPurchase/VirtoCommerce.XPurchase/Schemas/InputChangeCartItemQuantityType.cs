using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputChangeCartItemQuantityType : InputCartBaseType
    {
        public InputChangeCartItemQuantityType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId",
                "Line item Id");
            Field<NonNullGraphType<IntGraphType>>("quantity",
                "Quantity");
        }
    }
}
