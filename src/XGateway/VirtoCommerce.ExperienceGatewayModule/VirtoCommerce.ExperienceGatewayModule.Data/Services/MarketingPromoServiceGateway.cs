using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.XGateway.Core.Models;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule.Data.Services
{
    public class MarketingPromoServiceGateway : IMarketingPromoServiceGateway
    {
        private readonly IMarketingPromoEvaluator _marketingPromoEvaluator;

        public MarketingPromoServiceGateway(IMarketingPromoEvaluator marketingPromoEvaluator)
        {
            _marketingPromoEvaluator = marketingPromoEvaluator;
        }

        public string Gateway { get; set; } = Gateways.VirtoCommerce;

        public Task<PromotionResult> EvaluatePromotionAsync(IEvaluationContext context)
        {
            return _marketingPromoEvaluator.EvaluatePromotionAsync(context);
        }
    }
}
