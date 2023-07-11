using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
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
