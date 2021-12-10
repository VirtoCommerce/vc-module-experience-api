using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputRemoveWishlistItemType : InputObjectGraphType
    {
        public InputRemoveWishlistItemType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId", description: "List ID");
            Field<NonNullGraphType<StringGraphType>>("lineItemId", "Line item ID to remove");
        }
    }
}
