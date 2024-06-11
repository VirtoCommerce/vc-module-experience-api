using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputAddCouponType : InputCartBaseType
    {
        public InputAddCouponType()
        {
            Field<NonNullGraphType<StringGraphType>>("couponCode",
                "Coupon code");
        }
    }
}
