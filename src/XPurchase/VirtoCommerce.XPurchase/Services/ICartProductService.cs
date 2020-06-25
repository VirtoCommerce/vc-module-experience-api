using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Services
{
    public interface ICartProductService
    {
        Task<IEnumerable<CartProduct>> GetCartProductsByIdsAsync(ShoppingCart cart, string[] ids);
    }
}
