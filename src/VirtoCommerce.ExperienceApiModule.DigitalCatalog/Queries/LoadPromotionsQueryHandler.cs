using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadPromotionsQueryHandler : IQueryHandler<LoadPromotionsQuery, LoadPromotionsResponse>
    {
        private readonly IPromotionSearchServiceGateway _promotionSearchService;

        public LoadPromotionsQueryHandler(IPromotionSearchServiceGateway promotionSearchService)
        {
            _promotionSearchService = promotionSearchService;
        }

        public virtual async Task<LoadPromotionsResponse> Handle(LoadPromotionsQuery request, CancellationToken cancellationToken)
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
