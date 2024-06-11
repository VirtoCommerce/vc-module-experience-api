using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputAddWishlistItemType : InputObjectGraphType
    {
        public InputAddWishlistItemType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId", description: "Wish list id");
            Field<NonNullGraphType<StringGraphType>>("productId", description: "Product id to add");
            Field<IntGraphType>("quantity", description: "Product quantity to add");
        }
    }
}
