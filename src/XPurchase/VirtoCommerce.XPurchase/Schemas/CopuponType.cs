using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CopuponType : ObjectGraphType<CartCoupon>
    {
        public CopuponType()
        {
            Field(x => x.Code, nullable: true).Description("Coupon code");
            Field(x => x.IsAppliedSuccessfully, nullable: true).Description("Is coupon was applied successfully");
        }
    }
}
