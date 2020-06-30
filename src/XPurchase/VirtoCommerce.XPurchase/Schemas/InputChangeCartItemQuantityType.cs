using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputChangeCartItemQuantityType : InputCartBaseType
    {
        public InputChangeCartItemQuantityType()
        {
            Field<NonNullGraphType<StringGraphType>>("productId");
            Field<NonNullGraphType<IntGraphType>>("quantity");
        }
    }
}
