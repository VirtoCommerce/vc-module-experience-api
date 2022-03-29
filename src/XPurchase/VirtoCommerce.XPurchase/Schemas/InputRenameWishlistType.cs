using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputRenameWishlistType : InputObjectGraphType
    {
        public InputRenameWishlistType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId", description: "List ID");
            Field<StringGraphType>("listName", description: "New List name");
        }
    }
}
