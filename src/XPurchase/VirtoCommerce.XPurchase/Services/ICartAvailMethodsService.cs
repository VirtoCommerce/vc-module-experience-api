using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.XPurchase.Services
{
    public interface ICartAvailMethodsService
    {
        Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync(CartAggregate cartAggr);
        Task<IEnumerable<ShippingRate>> GetAvailableShippingRatesAsync(CartAggregate cartAggr);
    }
}
