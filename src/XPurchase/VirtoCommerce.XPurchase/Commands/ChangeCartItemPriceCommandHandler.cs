using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemPriceCommandHandler : CartCommandHandler<ChangeCartItemPriceCommand>
    {
        public ChangeCartItemPriceCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeCartItemPriceCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetOrCreateCartFromCommandAsync(request);
            await cartAggr.ChangeItemPriceAsync(new PriceAdjustment(request.ProductId, request.Price));
            await CartRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
    }
}
