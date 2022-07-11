using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Queries.Inventory
{
    public class SearchFulfillmentCentersQueryHandler : IRequestHandler<SearchFulfillmentCentersQuery, FulfillmentCenterSearchResult>
    {
        private readonly ISearchService<FulfillmentCenterSearchCriteria, FulfillmentCenterSearchResult, FulfillmentCenter> _fulfillmentCenterSearchService;
        private readonly ICrudService<Store> _storeService;

        public SearchFulfillmentCentersQueryHandler(
            ISearchService<FulfillmentCenterSearchCriteria, FulfillmentCenterSearchResult, FulfillmentCenter> fulfillmentCenterSearchService,
            ICrudService<Store> storeService)
        {
            _fulfillmentCenterSearchService = fulfillmentCenterSearchService;
            _storeService = storeService;
        }

        public async Task<FulfillmentCenterSearchResult> Handle(SearchFulfillmentCentersQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.StoreId))
            {
                var store = await _storeService.GetByIdAsync(request.StoreId);
                if (store != null)
                {
                    var fulfillmentCenterIds = new List<string>();

                    fulfillmentCenterIds.Add(store.MainFulfillmentCenterId);
                    fulfillmentCenterIds.Add(store.MainReturnsFulfillmentCenterId);
                    fulfillmentCenterIds.AddRange(store.AdditionalFulfillmentCenterIds);
                    fulfillmentCenterIds.AddRange(store.ReturnsFulfillmentCenterIds);

                    request.FulfillmentCenterIds = fulfillmentCenterIds.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToArray();
                }
            }

            var searchCriteria = new FulfillmentCenterSearchCriteria
            {
                Skip = request.Skip,
                Take = request.Take,
                Sort = request.Sort,
                Keyword = request.Query,
                ObjectIds = request.FulfillmentCenterIds,
            };

            var result = await _fulfillmentCenterSearchService.SearchAsync(searchCriteria);

            return result;
        }
    }
}
