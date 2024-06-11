using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Validators
{
    public class CartValidationContext
    {
        public CartAggregate CartAggregate { get; set; }
        public IEnumerable<CartProduct> AllCartProducts { get; set; } = Enumerable.Empty<CartProduct>();
        public IEnumerable<PaymentMethod> AvailPaymentMethods { get; set; } = Enumerable.Empty<PaymentMethod>();
        public IEnumerable<ShippingRate> AvailShippingRates { get; set; } = Enumerable.Empty<ShippingRate>();
    }
}
