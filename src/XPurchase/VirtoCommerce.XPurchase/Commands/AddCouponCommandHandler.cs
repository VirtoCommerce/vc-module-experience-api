using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCouponCommandHandler : CartCommandHandler<AddCouponCommand>
    {
        public AddCouponCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(AddCouponCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetCartAggregateFromCommandAsync(request);
            await cartAggr.AddCouponAsync(request.CouponCode);
            await CartAggrRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
    }
}
