using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule.Data.Services
{
    public class MarketingPromoServiceGateway : IMarketingPromoServiceGateway
    {
        private readonly IMarketingPromoEvaluator _marketingPromoEvaluator;

        public MarketingPromoServiceGateway(/*IMarketingPromoEvaluator marketingPromoEvaluator*/)
        {
            //_marketingPromoEvaluator = marketingPromoEvaluator;
        }

        public Task<PromotionResult> EvaluatePromotionAsync(IEvaluationContext context)
        {
            return _marketingPromoEvaluator.EvaluatePromotionAsync(context);
        }
    }
}
