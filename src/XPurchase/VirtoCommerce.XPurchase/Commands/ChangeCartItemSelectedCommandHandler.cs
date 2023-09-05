using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemSelectedCommandHandler : CartCommandHandler<ChangeCartItemSelectedCommand>
    {
        public ChangeCartItemSelectedCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeCartItemSelectedCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            var item = new ItemSelectedForCheckout(request.LineItemId, request.SelectedForCheckout);
            await cartAggregate.ChangeItemSelectedAsync(item);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
