using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputUpdateWishlistItemsType : InputObjectGraphType
    {
        public InputUpdateWishlistItemsType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId", description: "Wish list id");
            Field<NonNullGraphType<ListGraphType<InputUpdateWishlistLineItemType>>>("items", "Bulk wishlist items");
        }
    }
}
