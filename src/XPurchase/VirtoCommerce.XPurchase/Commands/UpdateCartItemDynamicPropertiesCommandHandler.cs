using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateCartItemDynamicPropertiesCommandHandler : CartCommandHandler<UpdateCartItemDynamicPropertiesCommand>
    {
        public UpdateCartItemDynamicPropertiesCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(UpdateCartItemDynamicPropertiesCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            await cartAggregate.UpdateCartItemDynamicProperties(request.LineItemId, request.DynamicProperties);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
