using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputChangeCartItemSelectedType : InputCartBaseType
    {
        public InputChangeCartItemSelectedType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId", "Line item Id");
            Field<NonNullGraphType<BooleanGraphType>>("selectedForCheckout", "Is item selected for checkout");
        }
    }
}
