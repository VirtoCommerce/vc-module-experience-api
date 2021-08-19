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

            if (request.IsGift)
            {
                // reset the price to 0
                product.ApplyPrices(new[] { new PricingModule.Core.Model.Price() {
                    Currency = cartAggregate.Currency.Code,
                    ProductId = product.Id,
                    List = 0 }
                }, cartAggregate.Currency);
            }

            await cartAggregate.AddItemAsync(new NewCartItem(request.ProductId, request.Quantity)
            {
                Comment = request.Comment,
                IsGift = request.IsGift,
                DynamicProperties = request.DynamicProperties,
                Price = request.Price,
                CartProduct = product
            });

            return await SaveCartAsync(cartAggregate);
        }
    }
}
