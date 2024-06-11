using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
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
