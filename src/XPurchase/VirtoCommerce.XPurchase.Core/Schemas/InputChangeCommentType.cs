using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
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
