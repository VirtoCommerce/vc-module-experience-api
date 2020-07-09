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
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            await cartAggregate.RemoveShipmentAsync(request.ShipmentId);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
