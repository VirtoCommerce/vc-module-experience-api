using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddWishlistBulkItemType : InputObjectGraphType
    {
        public InputAddWishlistBulkItemType()
        {
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("listIds", description: "Wish list ids");
            Field<NonNullGraphType<StringGraphType>>("productId", description: "Product id to add");
        }
    }
}
