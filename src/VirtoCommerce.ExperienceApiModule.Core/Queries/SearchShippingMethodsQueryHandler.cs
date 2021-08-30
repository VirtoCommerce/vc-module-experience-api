using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SearchShippingMethodsQueryHandler : IQueryHandler<SearchShippingMethodsQuery, ShippingMethodsSearchResult>
    {
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;

        public SearchShippingMethodsQueryHandler(IShippingMethodsSearchService shippingMethodsSearchService)
        {
            _shippingMethodsSearchService = shippingMethodsSearchService;
        }

        public virtual async Task<ShippingMethodsSearchResult> Handle(SearchShippingMethodsQuery request, CancellationToken cancellationToken)
        {
            var criteria = AbstractTypeFactory<ShippingMethodsSearchCriteria>.TryCreateInstance();

            criteria.StoreId = request.StoreId;
            criteria.IsActive = true;
            criteria.Skip = request.Skip;
            criteria.Take = request.Take;

            var searchResult = await _shippingMethodsSearchService.SearchShippingMethodsAsync(criteria);
            return searchResult;
        }
    }
}
