using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCartItemsCommandHandler : CartCommandHandler<RemoveCartItemsCommand>
    {
        public RemoveCartItemsCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RemoveCartItemsCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            await cartAggregate.RemoveItemsAsync(request.LineItemIds);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
