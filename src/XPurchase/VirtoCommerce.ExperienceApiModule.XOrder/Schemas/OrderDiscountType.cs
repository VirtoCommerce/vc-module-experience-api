using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderDiscountType : ObjectGraphType<Discount>
    {
        public OrderDiscountType()
        {
            Field<OrderMoneyType>("Amount", resolve: context => new Money(context.Source.DiscountAmount, context.OrderCurrency()));
            Field(x => x.Coupon, true);
            Field(x => x.Description, true);
            Field(x => x.PromotionId, true);
        }
    }
}
