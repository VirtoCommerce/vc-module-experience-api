using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Aggregates;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Models;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Factories
{
    public interface IShoppingCartAggregateFactory
    {
        Task<ShoppingCartAggregate> CreateOrGetShoppingCartAggregateAsync(ShoppingCartContext context);
    }
}
