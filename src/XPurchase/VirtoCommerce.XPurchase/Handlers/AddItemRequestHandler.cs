using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.XPurchase.Domain.Builders;
using VirtoCommerce.XPurchase.Domain.Factories;
using VirtoCommerce.XPurchase.Models.Cart.Services;
using VirtoCommerce.XPurchase.Models.Enums;
using VirtoCommerce.XPurchase.Models.OperationResults;
using VirtoCommerce.XPurchase.Requests;

namespace VirtoCommerce.XPurchase.Handlers
{
    /// <summary>
    /// Request handler for adding product item to cart
    /// </summary>
    public class AddItemRequestHandler : IRequestHandler<AddItemRequest, AddItemResponse>
    {
        private readonly IShoppingCartAggregateFactory _cartFactory;
        private readonly IProductsService _catalogService;

        public AddItemRequestHandler(IShoppingCartAggregateFactory cartFactory, IProductsService catalogService)
        {
            _cartFactory = cartFactory;
            _catalogService = catalogService;
        }

        public async Task<AddItemResponse> Handle(AddItemRequest request, CancellationToken cancellationToken)
        {
            request.CartItem.Type = request.Type;
            request.CartItem.ListName = request.CartName;

            var context = CartContextBuilder.Build()
                                            .WithStore(request.StoreId)
                                            .WithCartName(request.CartName)
                                            .WithUser(request.UserId)
                                            .WithCurrencyAndLanguage(request.CurrencyCode, request.CultureName)
                                            .WithCartType(request.Type)
                                            .GetContext();

            var cartAggregate = await _cartFactory
                .CreateOrGetShoppingCartAggregateAsync(context)
                .ConfigureAwait(false);

            var products = await _catalogService
                .GetProductsAsync(new[] { request.CartItem.ProductId }, context.Currency, context.Language, ItemResponseGroup.Inventory | ItemResponseGroup.ItemWithPrices)
                .ConfigureAwait(false);

            var product = products.FirstOrDefault();
            if (product == null)
            {
                return new AddItemResponse { ItemsQuantity = cartAggregate.Cart.ItemsQuantity };
            }

            request.CartItem.Product = product;

            var result = await cartAggregate.AddItemAsync(request.CartItem).ConfigureAwait(false);
            if (result is SuccessResult)
            {
                await cartAggregate.SaveAsync().ConfigureAwait(false);
            }

            return new AddItemResponse { ItemsQuantity = cartAggregate.Cart.ItemsQuantity };
        }
    }
}
