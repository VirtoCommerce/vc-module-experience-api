using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddWishlistItemCommandHandler : CartCommandHandler<AddWishlistItemCommand>
    {
        private readonly ICartProductService _cartProductService;

        public AddWishlistItemCommandHandler(ICartAggregateRepository cartAggrRepository, ICartProductService cartProductService)
            : base(cartAggrRepository)
        {
            _cartProductService = cartProductService;
        }

        public override async Task<CartAggregate> Handle(AddWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CartRepository.GetCartByIdAsync(request.ListId);
            var product = await GetdCartProduct(request.ProductId, cartAggregate);
            await cartAggregate.AddItemAsync(new NewCartItem(request.ProductId, 1)
            {
                CartProduct = product
            });

            return await SaveCartAsync(cartAggregate);
        }

        protected async Task<CartProduct> GetdCartProduct(string productId, CartAggregate cartAggregate)
        {
            return (await _cartProductService.GetCartProductsByIdsAsync(cartAggregate, new[] { productId })).FirstOrDefault();
        }
    }
}
