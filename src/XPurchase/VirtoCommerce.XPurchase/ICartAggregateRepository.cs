using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Domain.Aggregates;

namespace VirtoCommerce.XPurchase.Domain.Factories
{
    public interface ICartAggregateRepository
    {
        Task SaveAsync(Aggregates.CartAggregate cartAggregate);
        Task<Aggregates.CartAggregate> GetForCartAsync(ShoppingCart cart);
        Task<Aggregates.CartAggregate> GetOrCreateAsync(string cartName, string storeId, string userId, string language, string currency, string type = null);
    }
}
