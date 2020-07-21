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
                resolve: context =>
                {
                    var currency = context.GetCurrency();
                    return currency != null ? context.Source.DiscountAmount.ToMoney(currency) : null;
                });

            Field<MoneyType>(
                "amountWithTax",
                arguments: QueryArgumentPresets.ArgumentsForMoney(),
                resolve: context =>
                {
                    var currency = context.GetCurrency();
                    return currency != null ? context.Source.DiscountAmountWithTax.ToMoney(currency) : null;
                });
        }
    }
}
