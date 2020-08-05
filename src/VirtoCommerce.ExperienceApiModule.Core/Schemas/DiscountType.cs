using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class DiscountType : ObjectGraphType<Discount>
    {
        public DiscountType()
        {
            Field(x => x.Coupon, nullable: true).Description("Coupon");
            Field(x => x.Description, nullable: true).Description("Value of discount description");
            Field(x => x.PromotionId, nullable: true).Description("Value of promotion id");
            Field<DecimalGraphType>("amount", resolve: context => context.Source.DiscountAmount);
            Field<DecimalGraphType>("amountWithTax", resolve: context => context.Source.DiscountAmountWithTax);
        }
    }
}
