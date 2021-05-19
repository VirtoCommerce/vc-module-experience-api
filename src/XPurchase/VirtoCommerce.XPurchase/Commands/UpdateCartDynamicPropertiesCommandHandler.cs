using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateCartDynamicPropertiesCommandHandler : CartCommandHandler<UpdateCartDynamicPropertiesCommand>
    {
        public UpdateCartDynamicPropertiesCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(UpdateCartDynamicPropertiesCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            await cartAggregate.UpdateCartDynamicProperties(request.DynamicProperties);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
