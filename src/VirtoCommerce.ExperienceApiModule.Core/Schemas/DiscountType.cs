using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;

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
                resolve: context =>
                {
                    //TODO: We can't get currency here. Need to change amount to decimal type
                    return new Money(context.Source.DiscountAmount, new Currency());
                });

            Field<MoneyType>(
                "amountWithTax",
                resolve: context =>
                {
                    //TODO: We can't get currency here. Need to change amount to decimal type
                    return new Money(context.Source.DiscountAmountWithTax, new Currency());
                });
        }
    }
}
