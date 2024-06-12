using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.XPurchase.Core.Models
{
    public class PromotionEvaluationContextCartMap
    {
        public CartAggregate CartAggregate { get; set; }

        public PromotionEvaluationContext PromotionEvaluationContext { get; set; }
    }
}
