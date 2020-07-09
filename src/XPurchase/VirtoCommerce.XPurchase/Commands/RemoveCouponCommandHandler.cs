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
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            await cartAggregate.RemoveCouponAsync(request.CouponCode);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
