using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XPurchase.Queries;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Validators;
using CartAggregateBuilder = VirtoCommerce.XPurchase.AsyncObjectBuilder<VirtoCommerce.XPurchase.CartAggregate>;

namespace VirtoCommerce.XPurchase
{
    public class CartAggregateRepository : ICartAggregateRepository
    {
        private readonly Func<CartAggregate> _cartAggregateFactory;
        private readonly ICartProductService _cartProductsService;
        private readonly IShoppingCartSearchService _shoppingCartSearchService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICurrencyService _currencyService;
        private readonly IMemberResolver _memberResolver;
        private readonly IStoreService _storeService;

        public CartAggregateRepository(
            Func<CartAggregate> cartAggregateFactory,
            IShoppingCartSearchService shoppingCartSearchService,
            IShoppingCartService shoppingCartService,
            ICurrencyService currencyService,
            IMemberResolver memberResolver,
            IStoreService storeService,
            ICartProductService cartProductsService)
        {
            _cartAggregateFactory = cartAggregateFactory;
            _shoppingCartSearchService = shoppingCartSearchService;
            _shoppingCartService = shoppingCartService;
            _currencyService = currencyService;
            _memberResolver = memberResolver;
            _storeService = storeService;
            _cartProductsService = cartProductsService;
        }

        public async Task SaveAsync(CartAggregate cartAggregate)
        {
            await cartAggregate.RecalculateAsync();
            cartAggregate.Cart.ModifiedDate = DateTime.UtcNow;

            await _shoppingCartService.SaveChangesAsync(new List<ShoppingCart> { cartAggregate.Cart });
        }

        public async Task<CartAggregate> GetCartByIdAsync(string cartId, string language = null)
        {
            if (CartAggregateBuilder.IsBuilding(out var cartAggregate))
            {
                return cartAggregate;
            }

            var cart = await _shoppingCartService.GetByIdAsync(cartId);
            if (cart != null)
            {
                return await InnerGetCartAggregateFromCartAsync(cart, language ?? Language.InvariantLanguage.CultureName);
            }
            return null;
        }

        public Task<CartAggregate> GetCartForShoppingCartAsync(ShoppingCart cart, string language = null)
        {
            if (CartAggregateBuilder.IsBuilding(out var cartAggregate))
            {
                return Task.FromResult(cartAggregate);
            }

            return InnerGetCartAggregateFromCartAsync(cart, language ?? Language.InvariantLanguage.CultureName);
        }

        public async Task<CartAggregate> GetCartAsync(string cartName, string storeId, string userId, string language, string currencyCode, string type = null, string responseGroup = null)
        {
            if (CartAggregateBuilder.IsBuilding(out var cartAggregate))
            {
                return cartAggregate;
            }

            var criteria = new ShoppingCartSearchCriteria
            {
                StoreId = storeId,
                // IMPORTANT! Need to specify customerId, otherwise any user cart could be returned while we expect anonymous in this case.
                CustomerId = userId ?? AnonymousUser.UserName,
                Name = cartName,
                Currency = currencyCode,
                Type = type,
                ResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CartResponseGroup.Full).ToString()
            };

            var cartSearchResult = await _shoppingCartSearchService.SearchAsync(criteria);
            //The null value for the Type parameter should be interpreted as a valuable parameter, and we must return a cart object with Type property that has null exactly set.
            //otherwise, for the case where the system contains carts with different Types, the resulting cart may be a random result.
            var cart = cartSearchResult.Results.FirstOrDefault(x => (type != null) || x.Type == null);
            if (cart != null)
            {
                return await InnerGetCartAggregateFromCartAsync(cart.Clone() as ShoppingCart, language);
            }

            return null;
        }

        public async Task<SearchCartResponse> SearchCartAsync(ShoppingCartSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var searchResult = await _shoppingCartSearchService.SearchAsync(criteria);
            var cartAggregates = await GetCartsForShoppingCartsAsync(searchResult.Results);

            return new SearchCartResponse { Results = cartAggregates, TotalCount = searchResult.TotalCount };
        }

        public virtual Task RemoveCartAsync(string cartId) => _shoppingCartService.DeleteAsync(new[] { cartId }, softDelete: true);

        protected virtual async Task<CartAggregate> InnerGetCartAggregateFromCartAsync(ShoppingCart cart, string language, CartAggregateResponseGroup responseGroup = CartAggregateResponseGroup.Full)
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

            // Set Default Currency 
            if (string.IsNullOrEmpty(cart.Currency))
            {
                cart.Currency = store.DefaultCurrency;
            }
            // Actualize Cart Language From Context
            if (!string.IsNullOrEmpty(language) && cart.LanguageCode != language)
            {
                cart.LanguageCode = language;
            }

            language = !string.IsNullOrEmpty(cart.LanguageCode) ? cart.LanguageCode : store.DefaultLanguage;
            var currency = allCurrencies.GetCurrencyForLanguage(cart.Currency, language);

            var member = await _memberResolver.ResolveMemberByIdAsync(cart.CustomerId);
            var aggregate = _cartAggregateFactory();

            using (CartAggregateBuilder.Build(aggregate))
            {
                aggregate.GrabCart(cart, store, member, currency);


                //Load cart products explicitly if no validation is requested
                var cartProducts = await _cartProductsService.GetCartProductsByIdsAsync(aggregate, aggregate.Cart.Items.Select(x => x.ProductId).ToArray());
                //Populate aggregate.CartProducts with the  products data for all cart  line items
                foreach (var cartProduct in cartProducts)
                {
                    aggregate.CartProducts[cartProduct.Id] = cartProduct;
                }

                var validator = AbstractTypeFactory<CartLineItemPriceChangedValidator>.TryCreateInstance();

                foreach (var lineItem in aggregate.LineItems)
                {
                    var cartProduct = aggregate.CartProducts[lineItem.ProductId];
                    if (cartProduct == null)
                    {
                        continue;
                    }

                    await aggregate.SetItemFulfillmentCenterAsync(lineItem, cartProduct);
                    await aggregate.UpdateVendor(lineItem, cartProduct);
                    await aggregate.UpdateOrganization(cart, member);

                    // validate price change
                    var lineItemContext = new CartLineItemPriceChangedValidationContext
                    {
                        LineItem = lineItem,
                        CartProducts = aggregate.CartProducts,
                    };

                    var result = await validator.ValidateAsync(lineItemContext);
                    if (!result.IsValid)
                    {
                        aggregate.ValidationWarnings.AddRange(result.Errors);
                    }

                    // update price
                    aggregate.SetLineItemTierPrice(cartProduct.Price, lineItem.Quantity, lineItem);
                }

                await aggregate.RecalculateAsync();

                return aggregate;
            }
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
