using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class MergeCartType : InputCartBaseType
    {
        public MergeCartType()
        {
            Field<NonNullGraphType<StringGraphType>>("secondCartId");
        }
    }
}
