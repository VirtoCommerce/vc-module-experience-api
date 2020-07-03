using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputRemoveItemType : InputCartBaseType
    {
        public InputRemoveItemType()
        {
            Field<NonNullGraphType<StringGraphType>>("lineItemId");
        }
    }
}
