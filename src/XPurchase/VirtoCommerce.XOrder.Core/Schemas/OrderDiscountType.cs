using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XOrder.Core.Extensions;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class OrderDiscountType : ObjectGraphType<Discount>
    {
        public OrderDiscountType()
        {
            Field<NonNullGraphType<MoneyType>>("Amount",
                "Order discount amount",
                resolve: context => new Money(context.Source.DiscountAmount, context.GetOrderCurrency()));
            Field(x => x.Coupon, nullable: true);
            Field(x => x.Description, nullable: true);
            Field(x => x.PromotionId, nullable: true);
        }
    }
}
