using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddItemType : InputCartBaseType
    {
        public InputAddItemType()
        {
            Field<NonNullGraphType<StringGraphType>>("productId");
            Field<NonNullGraphType<IntGraphType>>("quantity");
            Field<DecimalGraphType>("price");
            Field<StringGraphType>("comment");
        }
    }
}
