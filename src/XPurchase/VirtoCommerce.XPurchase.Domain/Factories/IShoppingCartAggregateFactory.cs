using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Domain.Aggregates;
using VirtoCommerce.XPurchase.Domain.Models;

namespace VirtoCommerce.XPurchase.Domain.Factories
{
    public interface IShoppingCartAggregateFactory
    {
        Task<ShoppingCartAggregate> CreateOrGetShoppingCartAggregateAsync(ShoppingCartContext context);
    }
}
