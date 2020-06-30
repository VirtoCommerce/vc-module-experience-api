using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputDiscountType : InputObjectGraphType<Discount>
    {
        public InputDiscountType()
        {
            Field(x => x.Coupon, nullable: true).Description("Coupon");
            Field(x => x.Description, nullable: true).Description("Value of discount description");
            Field(x => x.PromotionId, nullable: true).Description("Value of promotion id");
            Field<InputMoneyType>("Amount", resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
        }
    }
}
