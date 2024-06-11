using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
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
