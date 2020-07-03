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
            var cartAggr = await GetOrCreateCartFromCommandAsync(request);
            await cartAggr.RemoveItemAsync(request.ProductId);
            await CartRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
    }
}
