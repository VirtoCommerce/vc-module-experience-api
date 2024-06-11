using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
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
