using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputChangeCartItemPriceType : InputCartBaseType
    {
        public InputChangeCartItemPriceType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId");
            Field<NonNullGraphType<DecimalGraphType>>("price");
        }
    }
}
