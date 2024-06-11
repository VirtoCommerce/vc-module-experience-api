using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputAddBulkItemsType : InputCartBaseType
    {
        public InputAddBulkItemsType()
        {
            Field<NonNullGraphType<ListGraphType<InputNewBulkItemType>>>("CartItems", "Bulk cart items");
        }
    }
}
