using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemsSelectedCommandHandler : CartCommandHandler<ChangeCartItemsSelectedCommand>
    {
        public ChangeCartItemsSelectedCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeCartItemsSelectedCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var item = new ItemSelectedForCheckout(request.LineItemIds, request.SelectedForCheckout);
            await cartAggregate.ChangeItemsSelectedAsync(item);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
