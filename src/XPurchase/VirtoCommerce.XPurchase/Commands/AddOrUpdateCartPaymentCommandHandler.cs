using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartPaymentCommandHandler : CartCommandHandler<AddOrUpdateCartPaymentCommand>
    {
        public AddOrUpdateCartPaymentCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(AddOrUpdateCartPaymentCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetCartAggregateFromCommandAsync(request);
            await cartAggr.AddOrUpdatePaymentAsync(request.Payment);
            await CartAggrRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
    }
}
