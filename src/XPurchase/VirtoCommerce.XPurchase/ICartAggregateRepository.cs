using System;
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

        Task<CartAggregate> GetCartAsync(ICartRequest cartRequest, string responseGroup = null);

        [Obsolete("Use GetCartAsync(ICartRequest cartRequest, string responseGroup)", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task<CartAggregate> GetCartAsync(string cartName, string storeId, string userId, string organizationId, string cultureName, string currencyCode, string type = null, string responseGroup = null);

        Task<CartAggregate> GetCartAsync(ShoppingCartSearchCriteria criteria, string cultureName);

        Task<CartAggregate> GetCartByIdAsync(string cartId, string cultureName = null);

        Task<CartAggregate> GetCartByIdAsync(string cartId, IList<string> productsIncludeFields, string cultureName = null);

        Task<CartAggregate> GetCartByIdAsync(string cartId, string responseGroup, IList<string> productsIncludeFields, string cultureName = null);

        Task<CartAggregate> GetCartForShoppingCartAsync(ShoppingCart cart, string cultureName = null);

        Task<SearchCartResponse> SearchCartAsync(ShoppingCartSearchCriteria criteria);

        Task<SearchCartResponse> SearchCartAsync(ShoppingCartSearchCriteria criteria, IList<string> productsIncludeFields);
    }
}
