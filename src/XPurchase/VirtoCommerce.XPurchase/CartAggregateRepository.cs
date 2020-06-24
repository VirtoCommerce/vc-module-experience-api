using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Domain.Factories
{
    public class CartAggregateRepository : ICartAggregateRepository
    {
        private readonly Func<Aggregates.CartAggregate> _cartAggregateFactory;
        private readonly IShoppingCartSearchService _shoppingCartSearchService;
        private readonly IShoppingCartService _shoppingCartService;
        public CartAggregateRepository(
            Func<Aggregates.CartAggregate> cartAggregateFactory
            , IShoppingCartSearchService shoppingCartSearchService
            , IShoppingCartService shoppingCartService)
        {
            _cartAggregateFactory = cartAggregateFactory;
            _shoppingCartSearchService = shoppingCartSearchService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task SaveAsync(Aggregates.CartAggregate cartAggregate)
        {
            await cartAggregate.RecalculateAsync();
            await _shoppingCartService.SaveChangesAsync(new ShoppingCart[] { cartAggregate.Cart });
        }

        public async Task<Aggregates.CartAggregate> GetForCartAsync(ShoppingCart cart)
        {
            var result = _cartAggregateFactory();

            await result.TakeCartAsync(cart);

            return result;

        }
        public async Task<Aggregates.CartAggregate> GetOrCreateAsync(string cartName, string storeId, string userId, string language, string currency, string type = null)
        {
            var criteria = new CartModule.Core.Model.Search.ShoppingCartSearchCriteria
            {
                StoreId = storeId,
                CustomerId = userId,
                Name = cartName,
                Currency = currency,
                Type = type
            };

            var cartSearchResult = await _shoppingCartSearchService.SearchCartAsync(criteria);

            var cart = cartSearchResult.Results.FirstOrDefault() ?? CreateCart(cartName, storeId, userId, language, currency, type);


            var aggregate = _cartAggregateFactory();

            await aggregate.TakeCartAsync(cart);

            return aggregate;
        }

        protected virtual ShoppingCart CreateCart(string cartName, string storeId, string userId, string languageCode, string currencyCode, string type)
        {
            var cart = AbstractTypeFactory<ShoppingCart>.TryCreateInstance();
            cart.CustomerId = userId;
            cart.Name = cartName;
            cart.StoreId = storeId;
            cart.LanguageCode = languageCode;
            cart.Type = type;
            //TODO:
            //cart.IsAnonymous = !user.IsRegisteredUser,
            //    CustomerName = user.IsRegisteredUser ? user.UserName : SecurityConstants.AnonymousUsername,
            //};

            return cart;
        }
    }
}
