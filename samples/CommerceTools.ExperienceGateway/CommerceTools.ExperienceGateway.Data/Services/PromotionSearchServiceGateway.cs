using System;
using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.XGateway.Core.Services;

namespace CommerceTools.ExperienceGateway.Data.Services
{
    public class PromotionSearchServiceGateway : IPromotionSearchServiceGateway
    {
        public Task<PromotionSearchResult> SearchPromotionsAsync(PromotionSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }
    }
}
