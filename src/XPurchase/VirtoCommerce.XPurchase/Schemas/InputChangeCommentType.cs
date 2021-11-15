using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputChangeCommentType : InputCartBaseType
    {
        public InputChangeCommentType()
        {
            Field<StringGraphType>("comment",
                "Comment");
        }
    }
}
