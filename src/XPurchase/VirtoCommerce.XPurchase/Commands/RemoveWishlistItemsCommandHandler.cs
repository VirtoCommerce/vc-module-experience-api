using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveWishlistItemsCommandHandler : CartCommandHandler<RemoveWishlistItemsCommand>
    {
        public RemoveWishlistItemsCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RemoveWishlistItemsCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CartRepository.GetCartByIdAsync(request.ListId);

            foreach (var lineItemId in request.LineItemIds)
            {
                await cartAggregate.RemoveItemAsync(lineItemId);
            }

            return await SaveCartAsync(cartAggregate);
        }
    }
}
