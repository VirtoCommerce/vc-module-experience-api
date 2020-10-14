using System;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.XGateway.Core.Services;

namespace CommerceTools.ExperienceGateway.Data.Services
{
    public class MarketingPromoServiceGateway : IMarketingPromoServiceGateway
    {
        public string Gateway { get; set; }

        public Task<PromotionResult> EvaluatePromotionAsync(IEvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
