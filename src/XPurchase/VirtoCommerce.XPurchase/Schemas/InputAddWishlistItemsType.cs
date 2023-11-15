using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddWishlistItemsType : InputObjectGraphType
    {
        public InputAddWishlistItemsType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId");
            Field<NonNullGraphType<ListGraphType<InputNewBulkItemType>>>("listItems", "Bulk list items");
        }
    }
}
