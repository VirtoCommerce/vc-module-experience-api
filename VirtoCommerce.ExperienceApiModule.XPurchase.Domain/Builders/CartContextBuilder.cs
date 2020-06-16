using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Models;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Security;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Stores;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Builders
{
    public class CartContextBuilder : ICartContextBuilder
    {
        private readonly ShoppingCartContext _context;

        protected CartContextBuilder()
        {
            _context = new ShoppingCartContext();
        }

        public static CartContextBuilder Build()
        {
            return new CartContextBuilder();
        }

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

        public ICartContextBuilder WithCurrency(Currency сurrency)
        {
            _context.Currency = сurrency;
            return this;
        }

        public ICartContextBuilder WithLanguage(Language language)
        {
            _context.Language = language;
            return this;
        }

        public ICartContextBuilder WithStore(Store store)
        {
            _context.CurrentStore = store;
            return this;
        }

        public ICartContextBuilder WithUser(User user)
        {
            _context.User = user;
            return this;
        }
    }
}
