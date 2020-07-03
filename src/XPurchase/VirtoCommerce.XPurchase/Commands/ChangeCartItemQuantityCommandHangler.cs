using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemQuantityCommandHangler : CartCommandHandler<ChangeCartItemQuantityCommand>
    {
        public ChangeCartItemQuantityCommandHangler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeCartItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetCartAggregateFromCommandAsync(request);
            await cartAggr.ChangeItemQuantityAsync(new ItemQtyAdjustment(request.LineItemId, request.Quantity));
            await CartAggrRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
    }
}
