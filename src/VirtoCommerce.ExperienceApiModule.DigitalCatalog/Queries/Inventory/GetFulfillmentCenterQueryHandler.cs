using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.XDigitalCatalog.Queries.Inventory
{
    internal class GetFulfillmentCenterQueryHandler : IQueryHandler<GetFulfillmentCenterQuery, FulfillmentCenter>
    {
        private readonly ICrudService<FulfillmentCenter> _fulfillmentCenterService;

        public GetFulfillmentCenterQueryHandler(IFulfillmentCenterService fulfillmentCenterService)
        {
            _fulfillmentCenterService = (ICrudService<FulfillmentCenter>)fulfillmentCenterService;
        }

        public async Task<FulfillmentCenter> Handle(GetFulfillmentCenterQuery request, CancellationToken cancellationToken)
        {
            var fulfillmentCenter = await _fulfillmentCenterService.GetByIdAsync(request.Id);

            return fulfillmentCenter;
        }
    }
}
