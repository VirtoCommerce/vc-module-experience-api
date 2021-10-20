using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Services
{
    public interface ICartProductService
    {
        /// <summary>
        /// Load products and fill their inventory data and prices based on specified cartAggr.
        /// </summary>
        /// <param name="cartAggr">Cart data to use</param>
        /// <param name="ids">Product ids</param>
        /// <returns></returns>
        Task<IEnumerable<CartProduct>> GetCartProductsByIdsAsync(CartAggregate cartAggr, string[] ids);
    }
}
