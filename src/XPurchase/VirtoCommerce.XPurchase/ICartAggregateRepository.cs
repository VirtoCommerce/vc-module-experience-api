using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.XPurchase.Queries;

namespace VirtoCommerce.XPurchase
{
    public interface ICartAggregateRepository
    {
        Task RemoveCartAsync(string cartId);

        Task SaveAsync(CartAggregate cartAggregate);

        Task<CartAggregate> GetCartAsync(string cartName, string storeId, string userId, string cultureName, string currencyCode, string type = null, string responseGroup = null);

        Task<CartAggregate> GetCartAsync(ShoppingCartSearchCriteria criteria, string cultureName);

        Task<CartAggregate> GetCartByIdAsync(string cartId, string language = null);

        Task<CartAggregate> GetCartForShoppingCartAsync(ShoppingCart cart, string cultureName = null);

        Task<SearchCartResponse> SearchCartAsync(ShoppingCartSearchCriteria criteria);
    }

    public interface ICartAggregateRepositoryExtended
    {
        Task<CartAggregate> GetCartByIdAsync(string cartId, IList<string> includeFields, string language = null);
    }
}
