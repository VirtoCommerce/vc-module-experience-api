using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputRemoveCartAddressType : InputCartBaseType
    {
        public InputRemoveCartAddressType()
        {
            Field<NonNullGraphType<StringGraphType>>("addressId",
                "Address Id");
        }
    }
}
