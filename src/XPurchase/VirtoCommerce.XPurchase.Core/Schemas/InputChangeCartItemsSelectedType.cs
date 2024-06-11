using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputChangeCartItemsSelectedType : InputCartBaseType
    {
        public InputChangeCartItemsSelectedType()
        {
            Field<ListGraphType<StringGraphType>>("lineItemIds", "List of line item Ids");
        }
    }
}
