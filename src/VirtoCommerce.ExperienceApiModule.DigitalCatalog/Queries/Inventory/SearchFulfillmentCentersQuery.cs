using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.InventoryModule.Core.Model.Search;

namespace VirtoCommerce.XDigitalCatalog.Queries.Inventory
{
    public class SearchFulfillmentCentersQuery : IQuery<FulfillmentCenterSearchResult>
    {
        public int Skip { get; set; }
        public int Take { get; set; }

        public string StoreId { get; set; }
        public string Query { get; set; }
        public string Sort { get; set; }

        public string[] FulfillmentCenterIds { get; set; }
    }
}
