using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class CopuponType : ObjectGraphType<Coupon>
    {
        public CopuponType()
        {
            Field(x => x.Code, nullable: true).Description("Coupon code");
            Field(x => x.Description, nullable: true).Description("Coupon description");
            Field(x => x.AppliedSuccessfully, nullable: true).Description("Is coupon was applied successfully");
            Field(x => x.ErrorCode, nullable: true).Description("Error code");
        }
    }
}
