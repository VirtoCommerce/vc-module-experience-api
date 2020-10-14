using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.XGateway.Core.Services
{
    public interface IMarketingPromoServiceGateway : IServiceGateway
    {
        Task<PromotionResult> EvaluatePromotionAsync(IEvaluationContext context);
    }
}
