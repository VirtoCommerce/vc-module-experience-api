using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddWishlistItemCommandHandler : CartCommandHandler<AddWishlistItemCommand>
    {
        public AddWishlistItemCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(AddWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CartRepository.GetCartByIdAsync(request.ListId);

            cartAggregate.ValidationRuleSet = new string[] { "default" };
            await cartAggregate.AddItemsAsync(new List<NewCartItem> {
                    new NewCartItem(request.ProductId, 1)
                });

            return await SaveCartAsync(cartAggregate);
        }
    }
}
