using System;
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
            var cartAggr = await GetOrCreateCartFromCommandAsync(request);
            await cartAggr.AddShipmentAsync(request.Shipment, await _cartAvailMethodService.GetAvailableShippingRatesAsync(cartAggr));
            await CartRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
    }
}
