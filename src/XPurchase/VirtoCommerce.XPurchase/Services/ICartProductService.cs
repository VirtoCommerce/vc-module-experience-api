using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Services
{
    public interface ICartProductService
    {
        /// <summary>
        /// Load products and fill their inventory data and prices based on specified <see cref="CartAggregate"/>
        /// </summary>
        /// <param name="aggregate">Cart data to use</param>
        /// <param name="ids">Product ids</param>
        /// <returns>List of <see cref="CartProduct"/></returns>
        Task<IList<CartProduct>> GetCartProductsByIdsAsync(CartAggregate aggregate, IList<string> ids);
    }
}
