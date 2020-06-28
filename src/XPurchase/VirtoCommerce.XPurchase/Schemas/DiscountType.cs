using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class DiscountType : ObjectGraphType<Discount>
    {
        public DiscountType()
        {
            Field(x => x.PromotionId, nullable: true).Description("Value of promotion id");
            //TODO: Convert to Money
            Field<MoneyType>("Amount", resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field(x => x.Coupon, nullable: true).Description("Coupon");
            Field(x => x.Description, nullable: true).Description("Value of discount description");
        }
    }
}
