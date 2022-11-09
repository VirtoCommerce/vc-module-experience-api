using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputUpdateWishlistItemType : InputObjectGraphType
    {
        public InputUpdateWishlistItemType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId", description: "Wish list id");
            Field<NonNullGraphType<StringGraphType>>("lineItemId", description: "Line Item Id to update");
            Field<NonNullGraphType<IntGraphType>>("quantity", description: "Product quantity to add");
        }
    }
}
