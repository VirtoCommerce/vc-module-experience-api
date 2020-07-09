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
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            await cartAggregate.AddCouponAsync(request.CouponCode);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
