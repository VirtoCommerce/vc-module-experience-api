using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries.Inventory
{
    public class SearchFulfillmentCentersQueryHandler : IRequestHandler<SearchFulfillmentCentersQuery, FulfillmentCenterSearchResult>
    {
        private readonly IFulfillmentCenterSearchService _fulfillmentCenterSearchService;
        private readonly IStoreService _storeService;

        public SearchFulfillmentCentersQueryHandler(
            IFulfillmentCenterSearchService fulfillmentCenterSearchService,
            IStoreService storeService)
        {
            _fulfillmentCenterSearchService = fulfillmentCenterSearchService;
            _storeService = storeService;
        }

        public async Task<FulfillmentCenterSearchResult> Handle(SearchFulfillmentCentersQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.StoreId))
            {
                var store = await _storeService.GetNoCloneAsync(request.StoreId);
                if (store != null)
                {
                    var fulfillmentCenterIds = new List<string>
                    {
                        store.MainFulfillmentCenterId,
                        store.MainReturnsFulfillmentCenterId,
                    };

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
