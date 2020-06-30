using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputChangeCartItemPriceType : InputCartBaseType
    {
        public InputChangeCartItemPriceType()
        {
            Field<NonNullGraphType<StringGraphType>>("productId");
            Field<NonNullGraphType<DecimalGraphType>>("price");
        }
    }
}
