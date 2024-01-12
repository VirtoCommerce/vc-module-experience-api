using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputRemoveWishlistItemsType : InputObjectGraphType
    {
        public InputRemoveWishlistItemsType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId", description: "List ID");
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>("lineItemIds", "Line item IDs to remove");
        }
    }
}
