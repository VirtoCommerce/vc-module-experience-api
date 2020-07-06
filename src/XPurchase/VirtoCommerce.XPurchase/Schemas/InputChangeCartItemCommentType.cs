using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputChangeCartItemCommentType : InputCartBaseType
    {
        public InputChangeCartItemCommentType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId");
            Field<NonNullGraphType<IntGraphType>>("comment");
        }
    }
}
