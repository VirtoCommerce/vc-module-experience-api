using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class DiscountType : ObjectGraphType<Discount>
    {
        public DiscountType()
        {
            Field(x => x.PromotionId, nullable: true).Description("Value of promotion id");
            Field<ObjectGraphType<MoneyType>>("Amount", resolve: context => context.Source.Amount);
            Field(x => x.Coupon, nullable: true).Description("Coupon");
            Field(x => x.Description, nullable: true).Description("Value of discount description");
        }
    }
}
