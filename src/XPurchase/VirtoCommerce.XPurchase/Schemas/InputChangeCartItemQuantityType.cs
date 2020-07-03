using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputChangeCartItemQuantityType : InputCartBaseType
    {
        public InputChangeCartItemQuantityType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId");
            Field<NonNullGraphType<IntGraphType>>("quantity");
        }
    }
}
