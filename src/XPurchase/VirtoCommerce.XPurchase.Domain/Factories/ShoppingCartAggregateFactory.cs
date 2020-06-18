using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Services;
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
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICatalogService _catalogService;
        private readonly IPromotionEvaluator _promotionEvaluator;
        private readonly ITaxEvaluator _taxEvaluator;
        private readonly ICartService _cartService;
        private readonly IShoppingCartSearchService _shoppingCartSearchService;

        public ShoppingCartAggregateFactory(IShoppingCartService shoppingCartService,
            ICatalogService catalogSearchService,
            IPromotionEvaluator promotionEvaluator,
            ITaxEvaluator taxEvaluator,
            ICartService cartService,
            IShoppingCartSearchService shoppingCartSearchService)
        {
            _shoppingCartService = shoppingCartService;
            _catalogService = catalogSearchService;
            _promotionEvaluator = promotionEvaluator;
            _taxEvaluator = taxEvaluator;
            _cartService = cartService;
            _shoppingCartSearchService = shoppingCartSearchService;
        }

        public async Task<ShoppingCartAggregate> CreateOrGetShoppingCartAggregateAsync(ShoppingCartContext context)
        {
            var aggregate = new ShoppingCartAggregate(_shoppingCartService, _catalogService, _promotionEvaluator, _taxEvaluator, _cartService, context);

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

            await aggregate.TakeCartAsync(cart);
            await aggregate.EvaluatePromotionsAsync();
            await aggregate.EvaluateTaxesAsync();

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
