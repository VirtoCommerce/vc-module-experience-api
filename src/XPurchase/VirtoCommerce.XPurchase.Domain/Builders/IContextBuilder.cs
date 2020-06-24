using VirtoCommerce.XPurchase.Domain.Models;

namespace VirtoCommerce.XPurchase.Domain.Builders
{
    public interface ICartContextBuilder
    {
        ICartContextBuilder WithDefaults();

        ShoppingCartContext GetContext();
    }
}
