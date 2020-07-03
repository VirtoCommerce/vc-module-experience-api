using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;
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
        private readonly IMemberService _memberService;
        private readonly IStoreService _storeService;

        public CartAggregateRepository(
            Func<CartAggregate> cartAggregateFactory
            , IShoppingCartSearchService shoppingCartSearchService
            , IShoppingCartService shoppingCartService
            , ICurrencyService currencyService
            , IMemberService memberService
            , IStoreService storeService
            , ICartValidationContextFactory cartValidationContextFactory)
        {
            _cartAggregateFactory = cartAggregateFactory;
            _shoppingCartSearchService = shoppingCartSearchService;
            _shoppingCartService = shoppingCartService;
            _currencyService = currencyService;
            _memberService = memberService;
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
            if(cart != null)
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

        protected virtual async Task<CartAggregate> InnerGetCartAggregateFromCartAsync(ShoppingCart cart, string language)
        {
            var store = await _storeService.GetByIdAsync(cart.StoreId);
            if (store == null)
            {
                throw new OperationCanceledException($"store with id {cart.StoreId} not found");
            }
            if (string.IsNullOrEmpty(cart.Currency))
            {
                cart.Currency = store.DefaultCurrency;
            }
            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();

            var currency = allCurrencies.FirstOrDefault(x => x.Code.EqualsInvariant(cart.Currency));
            if (currency == null)
            {
                throw new OperationCanceledException($"cart currency {cart.Currency} is not registered in the system");
            }
            //Clone  currency with cart language
            currency = new Currency(language != null ? new Language(language) : Language.InvariantLanguage, currency.Code, currency.Name, currency.Symbol, currency.ExchangeRate)
            {
                CustomFormatting = currency.CustomFormatting
            };

            var member = await _memberService.GetByIdAsync(cart.CustomerId);
            var aggregate = _cartAggregateFactory();
            await aggregate.TakeCartAsync(cart, store, member, currency);

            //Run validation
            await aggregate.ValidateAsync(await _cartValidationContextFactory.CreateValidationContextAsync(aggregate));

            return aggregate;
        }      
    }
}
