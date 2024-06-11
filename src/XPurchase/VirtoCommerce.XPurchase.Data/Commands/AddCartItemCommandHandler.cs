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

            return await SaveCartAsync(cartAggregate);
        }
    }
}
