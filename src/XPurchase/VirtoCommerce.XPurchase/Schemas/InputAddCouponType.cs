using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddCouponType : InputCartBaseType
    {
        public InputAddCouponType()
        {
            Field<NonNullGraphType<StringGraphType>>("couponCode");
        }
    }
}
