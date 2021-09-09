using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Services
{
    public interface ICartProductService
    {
        Task<IEnumerable<CartProduct>> GetCartProductsByIdsAsync(CartAggregate cartAggr, string[] ids, string additionalResponseGroups = null);
        Task<IEnumerable<CartProduct>> GetProductsByIdsAsync(CartAggregate cartAggr, string[] ids, string additionalResponseGroups = null);
    }
}
