using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.XDigitalCatalog.Queries.Inventory
{
    internal class GetFulfillmentCenterQueryHandler : IQueryHandler<GetFulfillmentCenterQuery, FulfillmentCenter>
    {
        private readonly ICrudService<FulfillmentCenter> _fulfillmentCenterService;

        public GetFulfillmentCenterQueryHandler(ICrudService<FulfillmentCenter> fulfillmentCenterService)
        {
            _fulfillmentCenterService = fulfillmentCenterService;
        }

        public async Task<FulfillmentCenter> Handle(GetFulfillmentCenterQuery request, CancellationToken cancellationToken)
        {
            var fulfillmentCenter = await _fulfillmentCenterService.GetByIdAsync(request.Id);

            return fulfillmentCenter;
        }
    }
}
