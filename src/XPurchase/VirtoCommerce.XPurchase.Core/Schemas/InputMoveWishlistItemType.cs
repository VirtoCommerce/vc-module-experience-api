using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputMoveWishlistItemType : InputObjectGraphType
    {
        public InputMoveWishlistItemType()
        {
            Field<NonNullGraphType<StringGraphType>>("listId", description: "Source List ID");
            Field<NonNullGraphType<StringGraphType>>("destinationListId", description: "Destination List ID");
            Field<NonNullGraphType<StringGraphType>>("lineItemId", "Line item ID to move");
        }
    }
}
