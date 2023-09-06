using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeAllCartItemsSelectedCommandHandler : CartCommandHandler<ChangeAllCartItemsSelectedCommand>
    {
        public ChangeAllCartItemsSelectedCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeAllCartItemsSelectedCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var items = cartAggregate.LineItems.Select(x => x.Id).ToList();
            var item = new ItemSelectedForCheckout(items, request.SelectedForCheckout);
            await cartAggregate.ChangeItemsSelectedAsync(item);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
