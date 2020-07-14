using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class DiscountType : ObjectGraphType<Discount>
    {
        public DiscountType()
        {
            Field(x => x.Coupon, nullable: true).Description("Coupon");
            Field(x => x.Description, nullable: true).Description("Value of discount description");
            Field(x => x.PromotionId, nullable: true).Description("Value of promotion id");
            Field<MoneyType>(
                "amount",
                arguments: QueryArgumentPresets.ArgumentsForMoney(),
                resolve: context => context.Source.DiscountAmount.ToMoney(context));
            Field<MoneyType>(
                "amountWithTax",
                arguments: QueryArgumentPresets.ArgumentsForMoney(),
                resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context));
        }
    }
}
