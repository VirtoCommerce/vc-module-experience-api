using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputRemoveCouponType : InputCartBaseType
    {
        public InputRemoveCouponType()
        {
            Field<StringGraphType>("couponCode",
                "Coupon code");
        }
    }
}
