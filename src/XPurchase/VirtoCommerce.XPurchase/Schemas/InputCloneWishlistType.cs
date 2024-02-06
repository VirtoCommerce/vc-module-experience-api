using GraphQL.Types;

namespace Opus.MainModule.ExperienceApi.Schemas;

public class InputCloneWishlistType : InputObjectGraphType
{
    public InputCloneWishlistType()
    {
        Field<NonNullGraphType<StringGraphType>>("storeId", description: "Store ID");
        Field<NonNullGraphType<StringGraphType>>("userId", description: "Owner ID");
        Field<NonNullGraphType<StringGraphType>>("listId", description: "ID of the cloned sheet");
        Field<StringGraphType>("listName", description: "List name");
        Field<StringGraphType>("cultureName", description: "Culture name");
        Field<StringGraphType>("currencyCode", description: "Currency code");
        Field<StringGraphType>("scope", description: "List scope (private or organization)");
        Field<StringGraphType>("description", description: "List description");
    }
}
