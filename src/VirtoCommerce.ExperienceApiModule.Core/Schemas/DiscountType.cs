using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class DiscountType : ObjectGraphType<Discount>
    {
        public DiscountType()
        {
            Field(x => x.Coupon, nullable: true).Description("Coupon");
            Field(x => x.Description, nullable: true).Description("Value of discount description");
            Field(x => x.PromotionId, nullable: true).Description("Value of promotion id");
            Field<MoneyType>("Amount", resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCurrency()));
        }
    }
}
