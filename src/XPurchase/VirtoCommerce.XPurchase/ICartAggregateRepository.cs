using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase
{
    public interface ICartAggregateRepository
    {
        Task RemoveCartAsync(string cartId);

        Task SaveAsync(CartAggregate cartAggregate);

        Task<CartAggregate> GetCartAsync(string cartName, string storeId, string userId, string language, string currencyCode, string type = null);

        Task<CartAggregate> GetCartByIdAsync(string cartId, string language = null);

        Task<CartAggregate> GetCartForShoppingCartAsync(ShoppingCart cart, string language = null);
    }
}
