using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Cart.Services
{
    public interface ICartService
    {
        //Task<IPagedList<ShoppingCart>> SearchCartsAsync(CartSearchCriteria criteria);
        //Task<ShoppingCart> SaveChanges(ShoppingCart cart);
        Task<ShoppingCart> GetByIdAsync(string cartId, Currency currency, string userId);

        Task DeleteCartByIdAsync(string cartId);

        Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync(ShoppingCart cart);

        Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync(ShoppingCart cart);
    }
}
