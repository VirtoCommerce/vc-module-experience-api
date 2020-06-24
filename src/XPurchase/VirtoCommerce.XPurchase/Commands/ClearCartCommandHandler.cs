using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Domain.Factories;

namespace VirtoCommerce.XPurchase.Domain.Commands
{
    public class ClearCartCommandHandler : CartCommandHandler<ClearCartCommand>
    {
        public ClearCartCommandHandler(ICartAggregateRepository cartAggrFactory)
            :base(cartAggrFactory)
        {
        }
        protected override async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await base.GetCartAggregateFromCommandAsync(request);
            await cartAggr.ClearAsync();
            await cartAggr.SaveAsync();
        }
    }
}
