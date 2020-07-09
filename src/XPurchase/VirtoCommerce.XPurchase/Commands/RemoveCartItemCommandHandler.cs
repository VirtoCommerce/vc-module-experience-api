using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCartItemCommandHandler : CartCommandHandler<RemoveCartItemCommand>
    {
        public RemoveCartItemCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            await cartAggregate.RemoveItemAsync(request.LineItemId);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
