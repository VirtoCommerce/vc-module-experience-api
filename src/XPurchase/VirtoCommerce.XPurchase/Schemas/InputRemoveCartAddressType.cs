using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputRemoveCartAddressType : InputCartBaseType
    {
        public InputRemoveCartAddressType()
        {
            Field<NonNullGraphType<StringGraphType>>("addressId");
        }
    }
}
