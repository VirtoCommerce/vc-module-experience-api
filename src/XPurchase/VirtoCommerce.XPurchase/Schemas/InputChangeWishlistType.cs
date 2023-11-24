using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputChangeWishlistType : InputObjectGraphType
    {
        public InputChangeWishlistType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId", description: "List ID");
            Field<StringGraphType>("listName", description: "New List name");
            Field<StringGraphType>("scope", description: "List scope (private or organization)");
            Field<StringGraphType>("description", description: "List description");
        }
    }
}
