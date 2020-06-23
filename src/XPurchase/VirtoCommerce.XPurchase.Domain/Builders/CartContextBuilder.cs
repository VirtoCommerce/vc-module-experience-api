using VirtoCommerce.XPurchase.Domain.Models;
using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Domain.Builders
{
    public class CartContextBuilder : ICartContextBuilder
    {
        private readonly ShoppingCartContext _context;

        protected CartContextBuilder() => _context = new ShoppingCartContext();

        public static CartContextBuilder Build() => new CartContextBuilder();

        public ShoppingCartContext GetContext() => _context;

        public ICartContextBuilder WithCartName(string сartName)
        {
            _context.CartName = сartName;
            return this;
        }

        public ICartContextBuilder WithCartType(string type)
        {
            _context.Type = type;
            return this;
        }

        public ICartContextBuilder WithCurrencyAndLanguage(string сurrencyCode, string cultureName)
        {
            var language = new Language(cultureName);
            var currency = new Currency(language, сurrencyCode); //todo add validation

            _context.Language = language;
            _context.Currency = currency;

            return this;
        }

        public ICartContextBuilder WithStore(string storeId)
        {
            _context.StoreId = storeId; //todo load store here
            return this;
        }

        public ICartContextBuilder WithUser(string userId)
        {
            _context.UserId = userId;
            return this;
        }
    }
}
