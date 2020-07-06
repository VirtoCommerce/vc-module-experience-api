using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveShipmentCommandHandler : CartCommandHandler<RemoveShipmentCommand>
    {
        public RemoveShipmentCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RemoveShipmentCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetOrCreateCartFromCommandAsync(request);
            await cartAggr.RemoveShipmentAsync(request.ShipmentId);
            await CartRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
    }
}
