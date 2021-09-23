using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RejectGiftCartItemsCommandHandler : CartCommandHandler<RejectGiftCartItemsCommand>
    {
        public RejectGiftCartItemsCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RejectGiftCartItemsCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            cartAggregate.RejectCartItems(request.Ids);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
