using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputValidateCouponType : InputCartBaseType
    {
        public InputValidateCouponType()
        {
            Field<InputCouponType>("coupon");
        }
    }
}
