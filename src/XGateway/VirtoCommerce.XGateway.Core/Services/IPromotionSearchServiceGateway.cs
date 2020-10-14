using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;

namespace VirtoCommerce.XGateway.Core.Services
{
    public interface IPromotionSearchServiceGateway : IServiceGateway
    {
        Task<PromotionSearchResult> SearchPromotionsAsync(PromotionSearchCriteria criteria);
    }
}
