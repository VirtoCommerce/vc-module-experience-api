using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartShipmentCommandHandler : CartCommandHandler<AddOrUpdateCartShipmentCommand>
    {
        private readonly ICartAvailMethodsService _cartAvailMethodService;
        private readonly IMapper _mapper;

        public AddOrUpdateCartShipmentCommandHandler(ICartAggregateRepository cartRepository, ICartAvailMethodsService cartAvailMethodService, IMapper mapper)
            : base(cartRepository)
        {
            _cartAvailMethodService = cartAvailMethodService;
            _mapper = mapper;
        }

        public override async Task<CartAggregate> Handle(AddOrUpdateCartShipmentCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var shipmentId = request.Shipment.Id?.Value ?? null;
            var shipment = cartAggregate.Cart.Shipments.FirstOrDefault(s => shipmentId != null && s.Id == shipmentId);
            shipment = _mapper.Map(request.Shipment, shipment);

            await cartAggregate.AddShipmentAsync(shipment, await _cartAvailMethodService.GetAvailableShippingRatesAsync(cartAggregate));

            return await SaveCartAsync(cartAggregate);
        }
    }
}
