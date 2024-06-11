using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputAddOrUpdateCartAddressType : InputCartBaseType
    {
        public InputAddOrUpdateCartAddressType()
        {
            Field<NonNullGraphType<InputAddressType>>("address",
                "Address");
        }
    }
}
