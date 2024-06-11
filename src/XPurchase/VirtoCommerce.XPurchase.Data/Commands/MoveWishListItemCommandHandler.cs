using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Models;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
{
    public class MoveWishListItemCommandHandler : CartCommandHandler<MoveWishlistItemCommand>
    {
        public MoveWishListItemCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(MoveWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var sourceCartAggregate = await CartRepository.GetCartByIdAsync(request.ListId);
            var destinationCartAggregate = await CartRepository.GetCartByIdAsync(request.DestinationListId);

            var item = sourceCartAggregate.Cart.Items.FirstOrDefault(x => x.Id == request.LineItemId);
            if (item != null)
            {
                destinationCartAggregate = await destinationCartAggregate.AddItemsAsync(new List<NewCartItem> {
                    new NewCartItem(item.ProductId, item.Quantity)
                });

                await sourceCartAggregate.RemoveItemAsync(request.LineItemId);

                await SaveCartAsync(destinationCartAggregate);
                await SaveCartAsync(sourceCartAggregate);
            }

            return destinationCartAggregate;
        }
    }
}
