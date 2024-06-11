using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputOrderDiscountType : InputObjectGraphType<Discount>
    {
        public InputOrderDiscountType()
        {
            Field(x => x.DiscountAmount);
            Field(x => x.Coupon, true);
            Field(x => x.Description, true);
            Field(x => x.PromotionId, true);
            Field(x => x.Id, true);
            Field(x => x.Currency);
            Field(x => x.DiscountAmountWithTax);
        }
    }
}
