using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddOrUpdateCartAddressType : InputCartBaseType
    {
        public InputAddOrUpdateCartAddressType()
        {
            Field<NonNullGraphType<InputAddressType>>("address");
        }
    }
}
