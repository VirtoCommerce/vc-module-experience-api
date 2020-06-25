using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase
{
    public interface ICartAggregateRepository
    {
        Task SaveAsync(CartAggregate cartAggregate);
        Task<CartAggregate> GetForCartAsync(ShoppingCart cart);
        Task<CartAggregate> GetOrCreateAsync(string cartName, string storeId, string userId, string language, string currency, string type = null);
    }
}
