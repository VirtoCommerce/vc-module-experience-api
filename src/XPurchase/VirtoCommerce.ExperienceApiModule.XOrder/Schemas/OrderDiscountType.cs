using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderDiscountType : ObjectGraphType<Discount>
    {
        public OrderDiscountType()
        {
            Field(x => x.Currency);
            Field(x => x.DiscountAmount);
            Field(x => x.DiscountAmountWithTax);
            Field(x => x.Coupon, true);
            Field(x => x.Description, true);
            Field(x => x.PromotionId, true);
        }
    }
}
