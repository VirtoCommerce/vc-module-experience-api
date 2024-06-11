using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
{
    public class ChangePurchaseOrderNumberCommandHandler : CartCommandHandler<ChangePurchaseOrderNumberCommand>
    {
        public ChangePurchaseOrderNumberCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangePurchaseOrderNumberCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            await cartAggregate.ChangePurchaseOrderNumber(request.PurchaseOrderNumber);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
