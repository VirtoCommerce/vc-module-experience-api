using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddWishlistBulkItemType : InputObjectGraphType
    {
        public InputAddWishlistBulkItemType()
        {
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("listIds", description: "List IDs");
            Field<NonNullGraphType<StringGraphType>>("productId", description: "Product ID to add");
        }
    }
}
