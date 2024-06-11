using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputChangeCartItemPriceType : InputCartBaseType
    {
        public InputChangeCartItemPriceType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId",
                "Line item Id");
            Field<NonNullGraphType<DecimalGraphType>>("price",
                "Price");
        }
    }
}
