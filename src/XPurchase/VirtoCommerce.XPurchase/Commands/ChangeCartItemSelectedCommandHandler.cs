using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemSelectedCommandHandler : CartCommandHandler<ChangeCartItemSelectedCommand>
    {
        public ChangeCartItemSelectedCommandHandler(ICartAggregateRepository cartAggregateRepository)
            : base(cartAggregateRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeCartItemSelectedCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            await cartAggregate.ChangeItemsSelectedAsync(new[] { request.LineItemId }, request.SelectedForCheckout);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
