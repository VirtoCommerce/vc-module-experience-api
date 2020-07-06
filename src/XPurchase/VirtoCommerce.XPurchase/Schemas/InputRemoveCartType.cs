using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputRemoveCartType : InputObjectGraphType
    {
        public InputRemoveCartType()
        {
            Field<NonNullGraphType<StringGraphType>>("cartId");
        }
    }
}
