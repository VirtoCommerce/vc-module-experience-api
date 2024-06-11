using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputRemoveItemsType : InputCartBaseType
    {
        public InputRemoveItemsType()
        {
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("lineItemIds",
                "Array of line item Id");
        }
    }
}
