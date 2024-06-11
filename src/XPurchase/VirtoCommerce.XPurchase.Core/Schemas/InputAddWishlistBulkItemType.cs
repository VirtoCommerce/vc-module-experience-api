using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputAddWishlistBulkItemType : InputObjectGraphType
    {
        public InputAddWishlistBulkItemType()
        {
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("listIds", description: "Wish list ids");
            Field<NonNullGraphType<StringGraphType>>("productId", description: "Product id to add");
            Field<IntGraphType>("quantity", description: "Product quantity to add");
        }
    }
}
