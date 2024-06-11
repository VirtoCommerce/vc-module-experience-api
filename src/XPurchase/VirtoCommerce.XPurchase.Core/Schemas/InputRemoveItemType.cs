using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputRemoveItemType : InputCartBaseType
    {
        public InputRemoveItemType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId",
                "Line item Id");
        }
    }
}
