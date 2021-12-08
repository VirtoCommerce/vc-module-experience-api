using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class MoveWishListItemCommandHandler : IRequestHandler<MoveWishlistItemCommand, CartAggregate>
    {
        private readonly ICartAggregateRepository _cartRepository;
        private readonly ICartProductService _cartProductService;

        public MoveWishListItemCommandHandler(ICartAggregateRepository cartAggrRepository, ICartProductService cartProductService)
        {
            _cartRepository = cartAggrRepository;
            _cartProductService = cartProductService;
        }

        public virtual async Task<CartAggregate> Handle(MoveWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var sourceCartAggregate = await _cartRepository.GetCartByIdAsync(request.ListId);
            var destinationCartAggregate = await _cartRepository.GetCartByIdAsync(request.DestinationListId);

            var item = sourceCartAggregate.Cart.Items.FirstOrDefault(x => x.Id == request.LineItemId);
            if (item != null)
            {
                var product = (await _cartProductService.GetCartProductsByIdsAsync(destinationCartAggregate, new[] { item.ProductId })).FirstOrDefault();
                await destinationCartAggregate.AddItemAsync(new NewCartItem(item.ProductId, item.Quantity)
                {
                    CartProduct = product
                });

                await sourceCartAggregate.RemoveItemAsync(request.LineItemId);

                await _cartRepository.SaveAsync(destinationCartAggregate);
                await _cartRepository.SaveAsync(sourceCartAggregate);
            }

            return destinationCartAggregate;
        }
    }
}
