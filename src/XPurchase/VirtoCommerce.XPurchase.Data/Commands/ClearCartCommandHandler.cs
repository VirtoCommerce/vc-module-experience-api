using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
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
