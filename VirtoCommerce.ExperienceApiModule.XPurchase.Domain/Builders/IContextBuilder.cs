using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Models;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Builders
{
    public interface ICartContextBuilder
    {
        ICartContextBuilder WithStore(string storeId);

        ICartContextBuilder WithCartName(string сartName);

        ICartContextBuilder WithUser(string userId);

        ICartContextBuilder WithCurrencyAndLanguage(string сurrencyCode, string cultureName);

        ICartContextBuilder WithCartType(string type);

        ShoppingCartContext GetContext();
    }
}
