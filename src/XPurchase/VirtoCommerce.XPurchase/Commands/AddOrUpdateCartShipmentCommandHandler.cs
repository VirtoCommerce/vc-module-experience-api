using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartShipmentCommandHandler : CartCommandHandler<AddOrUpdateCartShipmentCommand>
    {
        private readonly ICartAvailMethodsService _cartAvailMethodService;

        public AddOrUpdateCartShipmentCommandHandler(ICartAggregateRepository cartRepository, ICartAvailMethodsService cartAvailMethodService)
            : base(cartRepository)
        {
            _cartAvailMethodService = cartAvailMethodService;
        }

        public override async Task<CartAggregate> Handle(AddOrUpdateCartShipmentCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var shipmentId = request.Shipment.Id?.Value ?? null;
            var shipment = cartAggregate.Cart.Shipments.FirstOrDefault(s => shipmentId != null && s.Id == shipmentId);
            shipment = request.Shipment.MapTo(shipment);

            await cartAggregate.AddShipmentAsync(shipment, await _cartAvailMethodService.GetAvailableShippingRatesAsync(cartAggregate));

            return await SaveCartAsync(cartAggregate);
        }
    }
}
