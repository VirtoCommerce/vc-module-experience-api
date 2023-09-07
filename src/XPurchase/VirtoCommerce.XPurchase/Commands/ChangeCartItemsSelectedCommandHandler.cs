using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemsSelectedCommandHandler : CartCommandHandler<ChangeCartItemsSelectedCommand>
    {
        public ChangeCartItemsSelectedCommandHandler(ICartAggregateRepository cartAggregateRepository)
            : base(cartAggregateRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeCartItemsSelectedCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            await cartAggregate.ChangeItemsSelectedAsync(request.LineItemIds, request.SelectedForCheckout);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
