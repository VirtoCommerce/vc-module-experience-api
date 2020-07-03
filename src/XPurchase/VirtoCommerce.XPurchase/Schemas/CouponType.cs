using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CouponType : ObjectGraphType<CartCoupon>
    {
        public CouponType()
        {
            Field(x => x.Code, nullable: true).Description("Coupon code");
            Field(x => x.IsAppliedSuccessfully, nullable: true).Description("Is coupon was applied successfully");
        }
    }
}
