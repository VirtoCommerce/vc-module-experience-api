using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XPurchase.Queries;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.XPurchase
{
    public class CartAggregateRepository : ICartAggregateRepository
    {
        private readonly Func<CartAggregate> _cartAggregateFactory;
        private readonly ICartValidationContextFactory _cartValidationContextFactory;
        private readonly IShoppingCartSearchService _shoppingCartSearchService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICurrencyService _currencyService;
        private readonly IMemberResolver _memberResolver;
        private readonly IStoreService _storeService;

        public CartAggregateRepository(
            Func<CartAggregate> cartAggregateFactory
            , IShoppingCartSearchService shoppingCartSearchService
            , IShoppingCartService shoppingCartService
            , ICurrencyService currencyService
            , IMemberResolver memberResolver
            , IStoreService storeService
            , ICartValidationContextFactory cartValidationContextFactory
            )
        {
            _cartAggregateFactory = cartAggregateFactory;
            _shoppingCartSearchService = shoppingCartSearchService;
            _shoppingCartService = shoppingCartService;
            _currencyService = currencyService;
            _memberResolver = memberResolver;
            _storeService = storeService;
            _cartValidationContextFactory = cartValidationContextFactory;
        }

        public async Task SaveAsync(CartAggregate cartAggregate)
        {
            await cartAggregate.RecalculateAsync();
            await cartAggregate.ValidateAsync(await _cartValidationContextFactory.CreateValidationContextAsync(cartAggregate));
            await _shoppingCartService.SaveChangesAsync(new ShoppingCart[] { cartAggregate.Cart });
        }

        public async Task<CartAggregate> GetCartByIdAsync(string cartId, string language = null)
        {
            var cart = await _shoppingCartService.GetByIdAsync(cartId);
            if (cart != null)
            {
                return await InnerGetCartAggregateFromCartAsync(cart, language ?? Language.InvariantLanguage.CultureName);
            }
            return null;
        }        

        public async Task<CartAggregate> GetCartForShoppingCartAsync(ShoppingCart cart, string language = null)
        {
            return await InnerGetCartAggregateFromCartAsync(cart, language ?? Language.InvariantLanguage.CultureName);
        }

        public async Task<CartAggregate> GetCartAsync(string cartName, string storeId, string userId, string language, string currencyCode, string type = null)
        {
            var criteria = new CartModule.Core.Model.Search.ShoppingCartSearchCriteria
            {
                StoreId = storeId,
                CustomerId = userId,
                Name = cartName,
                Currency = currencyCode,
                Type = type
            };

            var cartSearchResult = await _shoppingCartSearchService.SearchCartAsync(criteria);
            var cart = cartSearchResult.Results.FirstOrDefault();
            if (cart != null)
            {
                return await InnerGetCartAggregateFromCartAsync(cart, language);
            }

            return null;
        }

        public async Task<SearchCartResponse> SearchCartAsync(ShoppingCartSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var searchResult = await _shoppingCartSearchService.SearchCartAsync(criteria);
            var cartAggregates = await GetCartsForShoppingCartsAsync(searchResult.Results);

            return new SearchCartResponse() { Results = cartAggregates, TotalCount = searchResult.TotalCount };
        }

        public virtual async Task RemoveCartAsync(string cartId) => await _shoppingCartService.DeleteAsync(new[] { cartId });

        protected virtual async Task<CartAggregate> InnerGetCartAggregateFromCartAsync(ShoppingCart cart, string language)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }

            var storeLoadTask = _storeService.GetByIdAsync(cart.StoreId);
            var allCurrenciesLoadTask = _currencyService.GetAllCurrenciesAsync();

            await Task.WhenAll(storeLoadTask, allCurrenciesLoadTask);

            var store = storeLoadTask.Result;
            var allCurrencies = allCurrenciesLoadTask.Result;

            if (store == null)
            {
                throw new OperationCanceledException($"store with id {cart.StoreId} not found");
            }
            if (string.IsNullOrEmpty(cart.Currency))
            {
                cart.Currency = store.DefaultCurrency;
            }

            var currency = allCurrencies.FirstOrDefault(x => x.Code.EqualsInvariant(cart.Currency));
            if (currency == null)
            {
                throw new OperationCanceledException($"cart currency {cart.Currency} is not registered in the system");
            }
            var defaultLanguage = store.DefaultLanguage != null ? new Language(store.DefaultLanguage) : Language.InvariantLanguage;
            //Clone  currency with cart language
            currency = new Currency(language != null ? new Language(language) : defaultLanguage, currency.Code, currency.Name, currency.Symbol, currency.ExchangeRate)
            {
                CustomFormatting = currency.CustomFormatting
            };

            var member = await _memberResolver.ResolveMemberByIdAsync(cart.CustomerId);
            var aggregate = _cartAggregateFactory();

            aggregate.GrabCart(cart, store, member, currency);

            var validationContext = await _cartValidationContextFactory.CreateValidationContextAsync(aggregate);
            //Populate aggregate.CartProducts with the  products data for all cart  line items
            foreach (var cartProduct in validationContext.AllCartProducts)
            {
                aggregate.CartProducts[cartProduct.Id] = cartProduct;
            }

            await aggregate.RecalculateAsync();

            //Run validation
            await aggregate.ValidateAsync(validationContext);

            return aggregate;
        }
      
        protected virtual async Task<IList<CartAggregate>> GetCartsForShoppingCartsAsync(IList<ShoppingCart> carts, string cultureName = null)
        {
            var result = new List<CartAggregate>();

            foreach (var shoppingCart in carts)
            {
                result.Add(await InnerGetCartAggregateFromCartAsync(shoppingCart, cultureName ?? Language.InvariantLanguage.CultureName));
            }

            return result;
        }
    }
}
