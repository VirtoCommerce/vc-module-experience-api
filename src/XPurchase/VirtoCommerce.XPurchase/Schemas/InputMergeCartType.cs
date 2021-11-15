using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputMergeCartType : InputCartBaseType
    {
        public InputMergeCartType()
        {
            Field<NonNullGraphType<StringGraphType>>("secondCartId",
                "Second cart Id");
        }
    }
}
