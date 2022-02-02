using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
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
