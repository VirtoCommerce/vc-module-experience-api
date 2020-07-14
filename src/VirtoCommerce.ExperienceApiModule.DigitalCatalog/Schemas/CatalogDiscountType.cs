using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Search;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class CatalogDiscountType : DiscountType
    {
        public CatalogDiscountType(IPromotionSearchService promotionSearchService)
        {
            FieldAsync<PromotionType>("promotion", resolve: async context => await ResolvePromotion(promotionSearchService, context.Source.PromotionId));
        }

        protected virtual async Task<Promotion> ResolvePromotion(IPromotionSearchService promotionSearchService, string promotionId)
        {
            var promotions = await promotionSearchService.SearchPromotionsAsync(new PromotionSearchCriteria
            {
                ObjectIds = new[] { promotionId },
            });

            return promotions.Results.FirstOrDefault();
        }
    }
}
