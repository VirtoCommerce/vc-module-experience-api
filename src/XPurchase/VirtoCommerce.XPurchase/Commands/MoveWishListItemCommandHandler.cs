using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class MoveWishListItemCommandHandler : CartCommandHandler<MoveWishlistItemCommand>
    {
        private readonly ICartProductService _cartProductService;

        public MoveWishListItemCommandHandler(ICartAggregateRepository cartAggrRepository, ICartProductService cartProductService)
            : base(cartAggrRepository)
        {
            _cartProductService = cartProductService;
        }

        public override async Task<CartAggregate> Handle(MoveWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var sourceCartAggregate = await CartRepository.GetCartByIdAsync(request.ListId);
            var destinationCartAggregate = await CartRepository.GetCartByIdAsync(request.DestinationListId);

            var item = sourceCartAggregate.Cart.Items.FirstOrDefault(x => x.Id == request.LineItemId);
            if (item != null)
            {
                var product = (await _cartProductService.GetCartProductsByIdsAsync(destinationCartAggregate, new[] { item.ProductId })).FirstOrDefault();
                await destinationCartAggregate.AddItemAsync(new NewCartItem(item.ProductId, item.Quantity)
                {
                    CartProduct = product
                });

                await sourceCartAggregate.RemoveItemAsync(request.LineItemId);

                await SaveCartAsync(destinationCartAggregate);
                await SaveCartAsync(sourceCartAggregate);
            }

            return destinationCartAggregate;
        }
    }
}
