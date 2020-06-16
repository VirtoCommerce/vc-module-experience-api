using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing.Services
{
    public interface IPromotionEvaluator
    {
        Task EvaluateDiscountsAsync(PromotionEvaluationContext promotionEvaluationContext, IEnumerable<IDiscountable> owners);
    }
}
