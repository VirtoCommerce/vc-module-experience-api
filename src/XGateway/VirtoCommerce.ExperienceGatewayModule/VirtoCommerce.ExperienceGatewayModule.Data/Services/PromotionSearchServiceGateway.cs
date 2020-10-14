using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.XGateway.Core.Models;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule.Data.Services
{
    public class PromotionSearchServiceGateway : IPromotionSearchServiceGateway
    {
        private readonly IPromotionSearchService _promotionSearchService;

        public PromotionSearchServiceGateway(IPromotionSearchService promotionSearchService)
        {
            _promotionSearchService = promotionSearchService;
        }

        public string Gateway { get; set; } = Gateways.VirtoCommerce;

        public Task<PromotionSearchResult> SearchPromotionsAsync(PromotionSearchCriteria criteria)
        {
            return _promotionSearchService.SearchPromotionsAsync(criteria);
        }
    }
}
