using System.Collections.Generic;
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

            var lineItemsIds = new List<string> { request.LineItemId };
            var item = new ItemSelectedForCheckout(lineItemsIds, request.SelectedForCheckout);
            await cartAggregate.ChangeItemsSelectedAsync(item);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
