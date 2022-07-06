using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Queries.Inventory
{
    public class GetFulfillmentCenterQuery : IQuery<FulfillmentCenter>
    {
        public string Id { get; set; }

        public string StoreId { get; set; }
    }
}
