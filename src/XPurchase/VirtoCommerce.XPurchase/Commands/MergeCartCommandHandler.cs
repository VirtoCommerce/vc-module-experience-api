using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class MergeCartCommandHandler : CartCommandHandler<MergeCartCommand>
    {
        public MergeCartCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(MergeCartCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetOrCreateCartFromCommandAsync(request);
            var secondCart = await GetCartById(request.SecondCartId, request.Language, request.Currency);
            if (secondCart != null)
            {
                cartAggr = await cartAggr.MergeWithCartAsync(secondCart);
                await CartRepository.SaveAsync(cartAggr);
            }

            return cartAggr;
        }
    }
}
