using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
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
            Field<DecimalGraphType>("amount", resolve: context => context.Source.DiscountAmount);
            Field<MoneyType>("moneyAmount", resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCurrencyByCode(context.Source.Currency)));
            Field<DecimalGraphType>("amountWithTax", resolve: context => context.Source.DiscountAmountWithTax);
            Field<MoneyType>("moneyAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCurrencyByCode(context.Source.Currency)));
        }
    }
}
