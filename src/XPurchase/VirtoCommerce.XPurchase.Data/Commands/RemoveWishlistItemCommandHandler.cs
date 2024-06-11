using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
{
    public class RemoveWishlistItemCommandHandler : CartCommandHandler<RemoveWishlistItemCommand>
    {
        public RemoveWishlistItemCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RemoveWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CartRepository.GetCartByIdAsync(request.ListId);

            if (!string.IsNullOrEmpty(request.LineItemId))
            {
                await cartAggregate.RemoveItemAsync(request.LineItemId);
            }

            if (!string.IsNullOrEmpty(request.ProductId))
            {
                await cartAggregate.RemoveItemsByProductIdAsync(request.ProductId);
            }

            return await SaveCartAsync(cartAggregate);
        }
    }
}
