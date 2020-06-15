using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Aggregates;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Models;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Security;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Stores;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Factories
{
    public class ShoppingCartAggregateFactory : IShoppingCartAggregateFactory
    {
        private readonly ICartService _cartService;
        private readonly ICatalogService _catalogService;
        private readonly IPromotionEvaluator _promotionEvaluator;
        private readonly ITaxEvaluator _taxEvaluator;

        public ShoppingCartAggregateFactory(ICartService cartService,
            ICatalogService catalogSearchService,
            IPromotionEvaluator promotionEvaluator,
            ITaxEvaluator taxEvaluator)
        {
            _cartService = cartService;
            _catalogService = catalogSearchService;
            _promotionEvaluator = promotionEvaluator;
            _taxEvaluator = taxEvaluator;
        }

        public async Task<ShoppingCartAggregate> CreateOrGetShoppingCartAggregateAsync(ShoppingCartContext context)
        {
            var aggregate = new ShoppingCartAggregate(_cartService, _catalogService, _promotionEvaluator, _taxEvaluator, context);

            var cartSearchCriteria = CreateCartSearchCriteria(context.CartName, context.CurrentStore, context.User, context.Language, context.Currency, context.Type);

            var cartSearchResult = await _cartService
                .SearchCartsAsync(cartSearchCriteria)
                .ConfigureAwait(false);

            var cart = cartSearchResult.FirstOrDefault()
                ?? CreateCart(context.CartName, context.CurrentStore, context.User, context.Language, context.Currency, context.Type);

            await aggregate.TakeCartAsync(cart);
            await aggregate.EvaluatePromotionsAsync();
            await aggregate.EvaluateTaxesAsync();

            return aggregate;
        }

        protected virtual CartSearchCriteria CreateCartSearchCriteria(string cartName, Store store, User user, Language language, Currency currency, string type)
        {
            return new CartSearchCriteria
            {
                StoreId = store.Id,
                Customer = user,
                Name = cartName,
                Currency = currency,
                Type = type
            };
        }

        protected virtual ShoppingCart CreateCart(string cartName, Store store, User user, Language language, Currency currency, string type)
        {
            var cart = new ShoppingCart(currency, language)
            {
                CustomerId = user.Id,
                Name = cartName,
                StoreId = store.Id,
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
