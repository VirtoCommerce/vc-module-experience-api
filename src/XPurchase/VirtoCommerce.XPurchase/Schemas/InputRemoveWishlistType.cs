using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputRemoveWishlistType : InputObjectGraphType
    {
        public InputRemoveWishlistType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId", description: "List ID");
        }
    }
}
