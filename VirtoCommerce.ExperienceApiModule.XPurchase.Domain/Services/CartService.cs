using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Converters;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Security;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Services
{
    public class CartService : ICartService
    {
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;

        public CartService(IPaymentMethodsSearchService paymentMethodsSearchService,
            IShoppingCartService shoppingCartService,
            IShippingMethodsSearchService shippingMethodsSearchService)
        {
            _paymentMethodsSearchService = paymentMethodsSearchService;
            _shoppingCartService = shoppingCartService;
            _shippingMethodsSearchService = shippingMethodsSearchService;
        }

        public async Task DeleteCartByIdAsync(string cartId)
            => await _shoppingCartService.DeleteAsync(new[] { cartId ?? throw new ArgumentNullException(nameof(cartId)) });

        public async Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync(ShoppingCart cart)
        {
            if (cart == null || string.IsNullOrEmpty(cart.StoreId) || cart.IsTransient())
            {
                return Enumerable.Empty<PaymentMethod>();
            }

            var criteria = new PaymentMethodsSearchCriteria
            {
                IsActive = true,
                Take = int.MaxValue,
                StoreId = cart.StoreId,
            };

            var payments = await _paymentMethodsSearchService.SearchPaymentMethodsAsync(criteria);

            return payments.Results.Select(x => x.ToCartPaymentMethod(cart)).OrderBy(x => x.Priority).ToList();
        }

        public virtual async Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync(ShoppingCart cart)
        {
            if (cart == null || string.IsNullOrEmpty(cart.StoreId) || cart.IsTransient())
            {
                return Enumerable.Empty<ShippingMethod>();
            }

            var criteria = new ShippingModule.Core.Model.Search.ShippingMethodsSearchCriteria
            {
                IsActive = true,
                Take = int.MaxValue,
                StoreId = cart.StoreId,
            };

            var shippingRates = await _shippingMethodsSearchService.SearchShippingMethodsAsync(criteria);

            return shippingRates.Results
                .Select(x => x.ToShippingMethod(cart.Currency))
                .OrderBy(x => x.Priority)
                .ToList();
        }

        public async Task<ShoppingCart> GetByIdAsync(string cartId, Currency currency, string userId)
        {
            var cartDto = await _shoppingCartService.GetByIdAsync(cartId, CartModule.Core.Model.CartResponseGroup.Full.ToString());
            if (cartDto == null)
            {
                return null;
            }

            var language = string.IsNullOrEmpty(cartDto.LanguageCode)
                ? Language.InvariantLanguage
                : new Language(cartDto.LanguageCode);

            return cartDto.ToShoppingCart(currency, language, new User { Id = userId });
        }

        // todo: maybe we need this in future
        //public virtual async Task<ShoppingCart> SaveChanges(ShoppingCart cart)
        //{
        //    if (cart == null)
        //    {
        //        throw new ArgumentNullException(nameof(cart));
        //    }
        //    var cartDto = cart.ToShoppingCartDto();
        //    if (string.IsNullOrEmpty(cartDto.Id))
        //    {
        //        cartDto = await _cartApi.CreateAsync(cartDto);
        //    }
        //    else
        //    {
        //        cartDto = await _cartApi.UpdateShoppingCartAsync(cartDto);
        //    }
        //    var result = cartDto.ToShoppingCart(cart.Currency, cart.Language, cart.Customer);
        //    return result;
        //}

        //public virtual async Task<IPagedList<ShoppingCart>> SearchCartsAsync(CartSearchCriteria criteria)
        //{
        //    if (criteria == null)
        //    {
        //        throw new ArgumentNullException(nameof(criteria));
        //    }

        //    var resultDto = await _cartApi.SearchShoppingCartAsync(criteria.ToSearchCriteriaDto());
        //    var result = new List<ShoppingCart>();
        //    foreach (var cartDto in resultDto.Results)
        //    {
        //        var currency = _workContextAccessor.WorkContext.AllCurrencies.FirstOrDefault(x => x.Equals(cartDto.Currency));
        //        var language = string.IsNullOrEmpty(cartDto.LanguageCode) ? Language.InvariantLanguage : new Language(cartDto.LanguageCode);
        //        var user = await _userManager.FindByIdAsync(cartDto.CustomerId) ?? criteria.Customer;
        //        var cart = cartDto.ToShoppingCart(currency, language, user);
        //        result.Add(cart);
        //    }

        //    return new StaticPagedList<ShoppingCart>(result, criteria.PageNumber, criteria.PageSize, resultDto.TotalCount.Value);

        //}
    }
}
