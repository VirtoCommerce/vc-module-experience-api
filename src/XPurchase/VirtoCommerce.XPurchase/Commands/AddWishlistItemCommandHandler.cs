using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddWishlistItemCommandHandler : IRequestHandler<AddWishlistItemCommand, CartAggregate>
    {
        private readonly ICartAggregateRepository _cartRepository;
        private readonly ICartProductService _cartProductService;

        public AddWishlistItemCommandHandler(ICartAggregateRepository cartAggrRepository, ICartProductService cartProductService)
        {
            _cartRepository = cartAggrRepository;
            _cartProductService = cartProductService;
        }

        public virtual async Task<CartAggregate> Handle(AddWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await _cartRepository.GetCartByIdAsync(request.ListId);

            var product = (await _cartProductService.GetCartProductsByIdsAsync(cartAggregate, new[] { request.ProductId })).FirstOrDefault();
            await cartAggregate.AddItemAsync(new NewCartItem(request.ProductId, 1)
            {
                CartProduct = product
            });

            await _cartRepository.SaveAsync(cartAggregate);
            return cartAggregate;
        }
    }
}
