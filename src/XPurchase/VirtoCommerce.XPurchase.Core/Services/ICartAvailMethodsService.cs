using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Services
{
    public interface ICartAvailMethodsService
    {
        Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync(CartAggregate cartAggr);
        Task<IEnumerable<ShippingRate>> GetAvailableShippingRatesAsync(CartAggregate cartAggr);
        Task<IEnumerable<GiftItem>> GetAvailableGiftsAsync(CartAggregate cartAggr);
    }
}
