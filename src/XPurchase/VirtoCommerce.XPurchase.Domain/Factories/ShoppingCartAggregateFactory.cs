using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.XPurchase.Domain.Aggregates;
using VirtoCommerce.XPurchase.Domain.Converters;
using VirtoCommerce.XPurchase.Domain.Models;
using VirtoCommerce.XPurchase.Models.Cart;
using VirtoCommerce.XPurchase.Models.Cart.Services;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.Marketing.Services;
using VirtoCommerce.XPurchase.Models.Security;

namespace VirtoCommerce.XPurchase.Domain.Factories
{
    public class ShoppingCartAggregateFactory : IShoppingCartAggregateFactory
    {
        private readonly IProductsRepository _catalogService;
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;
        private readonly IPromotionEvaluator _promotionEvaluator;
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;
        private readonly IShoppingCartSearchService _shoppingCartSearchService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITaxEvaluator _taxEvaluator;

        public ShoppingCartAggregateFactory(IProductsRepository catalogSearchService,
            IPaymentMethodsSearchService paymentMethodsSearchService,
            IPromotionEvaluator promotionEvaluator,
            IShippingMethodsSearchService shippingMethodsSearchService,
            IShoppingCartSearchService shoppingCartSearchService,
            IShoppingCartService shoppingCartService,
            ITaxEvaluator taxEvaluator)
        {
            _shoppingCartService = shoppingCartService;
            _catalogService = catalogSearchService;
            _promotionEvaluator = promotionEvaluator;
            _taxEvaluator = taxEvaluator;
            _paymentMethodsSearchService = paymentMethodsSearchService;
            _shippingMethodsSearchService = shippingMethodsSearchService;
            _shoppingCartSearchService = shoppingCartSearchService;
        }

        public async Task<ShoppingCartAggregate> CreateOrGetShoppingCartAggregateAsync(ShoppingCartContext context)
        {
            var criteria = new CartModule.Core.Model.Search.ShoppingCartSearchCriteria
            {
                StoreId = context.StoreId,
                CustomerId = context.UserId,
                Name = context.CartName,
                Type = context.Type
            };

            var cartSearchResult = await _shoppingCartSearchService.SearchCartAsync(criteria).ConfigureAwait(false);

            var user = new User
            {
                Id = context.UserId
            }; //await _userManager.FindByIdAsync(context.UserId); //todo get from user manager

            var cart = cartSearchResult.Results.FirstOrDefault()?.ToShoppingCart(context.Currency, context.Language, user)
                ?? CreateCart(context.CartName, context.StoreId, user, context.Language, context.Currency, context.Type);

            var aggregate = new ShoppingCartAggregate(
                _catalogService,
                _paymentMethodsSearchService,
                _promotionEvaluator,
                _shippingMethodsSearchService,
                _shoppingCartService,
                _taxEvaluator,
                context);

            await aggregate.TakeCartAsync(cart);

            return aggregate;
        }

        protected virtual ShoppingCart CreateCart(string cartName, string storeId, User user, Language language, Currency currency, string type)
        {
            var cart = new ShoppingCart(currency, language)
            {
                CustomerId = user.Id,
                Name = cartName,
                StoreId = storeId,
                Language = language,
                Customer = user,
                Type = type,
                IsAnonymous = !user.IsRegisteredUser,
                CustomerName = user.IsRegisteredUser ? user.UserName : SecurityConstants.AnonymousUsername,
            };

            return cart;
        }
    }
}
