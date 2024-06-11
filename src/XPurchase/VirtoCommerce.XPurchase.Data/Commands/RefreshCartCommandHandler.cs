using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
{
    /// <summary>
    /// Loads the cart from its persistent storage, then immediately saves it back without any modifications. This effectively triggers any relevant price updates and clears any warnings or errors that may have been present.
    /// </summary>
    public class RefreshCartCommandHandler : CartCommandHandler<RefreshCartCommand>
    {
        public RefreshCartCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RefreshCartCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
