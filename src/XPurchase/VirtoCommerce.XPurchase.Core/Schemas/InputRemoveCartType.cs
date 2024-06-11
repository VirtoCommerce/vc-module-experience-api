using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputRemoveCartType : InputObjectGraphType
    {
        public InputRemoveCartType()
        {
            Field<NonNullGraphType<StringGraphType>>("cartId", "Cart Id");
            Field<NonNullGraphType<StringGraphType>>("userId", "User Id");
        }
    }
}
