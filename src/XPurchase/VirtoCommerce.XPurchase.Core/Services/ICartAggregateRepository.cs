using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Services
{
    public interface ICartAggregateRepository
    {
        Task RemoveCartAsync(string cartId);

        Task SaveAsync(CartAggregate cartAggregate);

        Task<CartAggregate> GetCartAsync(string cartName, string storeId, string userId, string cultureName, string currencyCode, string type = null, string responseGroup = null);

        Task<CartAggregate> GetCartAsync(ShoppingCartSearchCriteria criteria, string cultureName);

        Task<CartAggregate> GetCartByIdAsync(string cartId, string language = null);

        Task<CartAggregate> GetCartByIdAsync(string cartId, IList<string> productsIncludeFields, string language = null);

        Task<CartAggregate> GetCartForShoppingCartAsync(ShoppingCart cart, string cultureName = null);

        Task<SearchCartResponse> SearchCartAsync(ShoppingCartSearchCriteria criteria);

        Task<SearchCartResponse> SearchCartAsync(ShoppingCartSearchCriteria criteria, IList<string> productsIncludeFields);
    }
}
