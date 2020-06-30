using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputCouponType : InputObjectGraphType<CartCoupon>
    {
        public InputCouponType()
        {
            Field(x => x.Code, nullable: false).Description("Coupon code");
        }
    }
}
