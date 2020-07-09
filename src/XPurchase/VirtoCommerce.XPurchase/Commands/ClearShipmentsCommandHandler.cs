using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearShipmentsCommandHandler : CartCommandHandler<ClearShipmentsCommand>
    {
        public ClearShipmentsCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ClearShipmentsCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await GetOrCreateCartFromCommandAsync(request);
            if (aggregate == null)
            {
                return null;
            }

            aggregate.Cart.Shipments.Clear();

            return await SaveCartAsync(aggregate);
        }
    }
}
