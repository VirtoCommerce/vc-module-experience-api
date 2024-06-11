using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputMergeCartType : InputCartBaseType
    {
        public InputMergeCartType()
        {
            Field<NonNullGraphType<StringGraphType>>("secondCartId", "Second cart Id");
            Field<BooleanGraphType>("deleteAfterMerge", "Delete second cart after merge");
        }
    }
}
