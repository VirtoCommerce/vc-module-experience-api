using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateCartShipmentDynamicPropertiesCommandHandler : CartCommandHandler<UpdateCartShipmentDynamicPropertiesCommand>
    {
        public UpdateCartShipmentDynamicPropertiesCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(UpdateCartShipmentDynamicPropertiesCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            await cartAggregate.UpdateCartShipmentDynamicProperties(request.ShipmentId, request.DynamicProperties);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
