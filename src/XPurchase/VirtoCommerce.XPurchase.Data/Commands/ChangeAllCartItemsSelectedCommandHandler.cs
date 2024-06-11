using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
{
    public class ChangeAllCartItemsSelectedCommandHandler : CartCommandHandler<ChangeAllCartItemsSelectedCommand>
    {
        public ChangeAllCartItemsSelectedCommandHandler(ICartAggregateRepository cartAggregateRepository)
            : base(cartAggregateRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeAllCartItemsSelectedCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var lineItemIds = cartAggregate.LineItems.Select(x => x.Id).ToArray();
            await cartAggregate.ChangeItemsSelectedAsync(lineItemIds, request.SelectedForCheckout);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
