using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RejectCartItemsCommandHandler : CartCommandHandler<RejectCartItemsCommand>
    {
        public RejectCartItemsCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RejectCartItemsCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            cartAggregate.RejectCartItems(request.GiftItemIds);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
