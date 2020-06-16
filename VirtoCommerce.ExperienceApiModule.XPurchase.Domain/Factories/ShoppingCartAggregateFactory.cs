using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Aggregates;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Converters;
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
                StoreId = context.CurrentStore.Id,
                CustomerId = context.User?.Id,
                Name = context.CartName,
                Type = context.Type
            };

            var cartSearchResult = await _shoppingCartSearchService.SearchCartAsync(criteria).ConfigureAwait(false);

            var cart = cartSearchResult.Results.FirstOrDefault()?.ToShoppingCart(context.Currency,context.Language,context.User)
                ?? CreateCart(context.CartName, context.CurrentStore, context.User, context.Language, context.Currency, context.Type);

            await aggregate.TakeCartAsync(cart);
            await aggregate.EvaluatePromotionsAsync();
            await aggregate.EvaluateTaxesAsync();

            return aggregate;
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
