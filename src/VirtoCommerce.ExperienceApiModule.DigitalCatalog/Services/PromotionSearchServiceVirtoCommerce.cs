using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class PromotionSearchServiceVirtoCommerce : IPromotionSearchServiceGateway, IService
    {
        private readonly IPromotionSearchService _promotionSearchService;

        public PromotionSearchServiceVirtoCommerce(IPromotionSearchService promotionSearchService)
        {
            _promotionSearchService = promotionSearchService;
        }

        public string Provider { get; set; } = Providers.PlatformModule;

        public async Task<LoadPromotionsResponse> SearchPromotionsAsync(LoadPromotionsQuery request)
        {
            var promotions = await _promotionSearchService.SearchPromotionsAsync(new PromotionSearchCriteria
            {
                ObjectIds = request.Ids.ToArray(),
            });

            return new LoadPromotionsResponse
            {
                Promotions = promotions.Results.ToDictionary(x => x.Id)
            };
        }
    }
}
