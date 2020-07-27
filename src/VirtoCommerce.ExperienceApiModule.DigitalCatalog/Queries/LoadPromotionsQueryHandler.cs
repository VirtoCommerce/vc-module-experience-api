using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Search;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadPromotionsQueryHandler : IQueryHandler<LoadPromotionsQuery, LoadPromotionsResponce>
    {
        private readonly IPromotionSearchService _promotionSearchService;

        public LoadPromotionsQueryHandler(IPromotionSearchService promotionSearchService)
        {
            _promotionSearchService = promotionSearchService;
        }

        public virtual async Task<LoadPromotionsResponce> Handle(LoadPromotionsQuery request, CancellationToken cancellationToken)
        {
            var promotions = await _promotionSearchService.SearchPromotionsAsync(new PromotionSearchCriteria
            {
                ObjectIds = request.Ids.ToArray(),
            });

            return new LoadPromotionsResponce
            {
                Promotions = promotions.Results.ToDictionary(x => x.Id)
            };
        }
    }
}
