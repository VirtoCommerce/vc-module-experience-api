using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearCartCommandHandler : CartCommandHandler<ClearCartCommand>
    {
        public ClearCartCommandHandler(ICartAggregateRepository cartAggrFactory)
            : base(cartAggrFactory)
        {
        }

        public override async Task<CartAggregate> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            await cartAggregate.ClearAsync();

            return await SaveCartAsync(cartAggregate);
        }
    }
}
