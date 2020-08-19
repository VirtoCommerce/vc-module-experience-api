using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Queries;

namespace VirtoCommerce.XPurchase
{
    public interface ICartAggregateRepository
    {
        Task RemoveCartAsync(string cartId);

        Task SaveAsync(CartAggregate cartAggregate);

        Task<CartAggregate> GetCartAsync(string cartName, string storeId, string userId, string cultureName, string currencyCode, string type = null);

        Task<CartAggregate> GetCartByIdAsync(string cartId, string language = null);

        Task<CartAggregate> GetCartForShoppingCartAsync(ShoppingCart cart, string cultureName = null);

        ShoppingCart CreateDefaultShoppingCart<TCartCommand>(TCartCommand request) where TCartCommand : CartCommand;

        Task<SearchCartResponse> SearchCartAsync(string storeId, string userId, string cultureName, string currencyCode, string type, string sort, int skip, int take);
    }
}
