using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartItemCommandHandler : CartCommandHandler<AddCartItemCommand>
    {
        private readonly ICartProductService _cartProductService;

        public AddCartItemCommandHandler(ICartAggregateRepository cartRepository, ICartProductService cartProductService)
            : base(cartRepository)
        {
            _cartProductService = cartProductService;
        }

        public override async Task<CartAggregate> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            var product = (await _cartProductService.GetCartProductsByIdsAsync(cartAggregate, new[] { request.ProductId })).FirstOrDefault();
            await cartAggregate.AddItemAsync(new NewCartItem(request.ProductId, request.Quantity)
            {
                Comment = request.Comment,
                DynamicProperties = request.DynamicProperties,
                Price = request.Price,
                CartProduct = product
            });

            // Prevent saving invalid cart
            if (!cartAggregate.IsValid)
            {
                return cartAggregate;
            }

            return await SaveCartAsync(cartAggregate);
        }
    }
}
