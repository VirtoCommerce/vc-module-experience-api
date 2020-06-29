using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCouponCommandHandler : CartCommandHandler<RemoveCouponCommand>
    {
        public RemoveCouponCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RemoveCouponCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetCartAggregateFromCommandAsync(request);
            await cartAggr.RemoveCouponAsync(request.CouponCode);
            await CartAggrRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
    }
}
