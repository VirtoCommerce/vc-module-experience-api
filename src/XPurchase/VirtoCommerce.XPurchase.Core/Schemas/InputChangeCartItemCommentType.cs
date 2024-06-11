using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputChangeCartItemCommentType : InputCartBaseType
    {
        public InputChangeCartItemCommentType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId",
                "Line item Id");
            Field<NonNullGraphType<StringGraphType>>("comment",
                "Comment");
        }
    }
}
