using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XDigitalCatalog.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadPromotionsQueryHandler : IQueryHandler<LoadPromotionsQuery, LoadPromotionsResponse>
    {
        private readonly IPromotionSearchServiceGateway _promotionSearchService;

        public LoadPromotionsQueryHandler(IPromotionSearchServiceGateway promotionSearchService)
        {
            _promotionSearchService = promotionSearchService;
        }

        public virtual Task<LoadPromotionsResponse> Handle(LoadPromotionsQuery request, CancellationToken cancellationToken)
        {
            return _promotionSearchService.SearchPromotionsAsync(request);
        }
    }
}
