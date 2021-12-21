using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddWishlistItemType : InputObjectGraphType
    {
        public InputAddWishlistItemType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId", description: "List ID");
            Field<NonNullGraphType<StringGraphType>>("productId", description: "Product ID to add");
        }
    }
}
