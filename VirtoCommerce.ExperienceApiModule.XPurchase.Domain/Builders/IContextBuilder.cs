using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Models;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Security;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Stores;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Builders
{
    public interface ICartContextBuilder
    {
        ICartContextBuilder WithStore(Store store);

        ICartContextBuilder WithCartName(string сartName);

        ICartContextBuilder WithUser(User user);

        ICartContextBuilder WithLanguage(Language language);

        ICartContextBuilder WithCurrency(Currency сurrency);

        ICartContextBuilder WithCartType(string type);

        ShoppingCartContext GetContext();
    }
}
