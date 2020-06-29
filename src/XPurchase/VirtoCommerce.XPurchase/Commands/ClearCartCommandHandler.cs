using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearCartCommandHandler : CartCommandHandler<ClearCartCommand>
    {
        public ClearCartCommandHandler(ICartAggregateRepository cartAggrFactory)
            :base(cartAggrFactory)
        {
        }

        public override async Task<CartAggregate> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetCartAggregateFromCommandAsync(request);
            await cartAggr.ClearAsync();
            await CartAggrRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
       
    }
}
