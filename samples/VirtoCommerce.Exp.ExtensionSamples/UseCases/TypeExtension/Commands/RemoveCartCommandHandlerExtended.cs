using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Commands;

namespace VirtoCommerce.Exp.ExtensionSamples.Commands
{
    public class RemoveCartCommandHandlerExtended : RemoveCartCommandHandler
    {
        public RemoveCartCommandHandlerExtended(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<bool> Handle(RemoveCartCommand request, CancellationToken cancellationToken)
        {
            var result = await base.Handle(request, cancellationToken);
            return result && request is RemoveCartCommandExtended requestExtended && !string.IsNullOrEmpty(requestExtended.Reason);
        }
    }
}
