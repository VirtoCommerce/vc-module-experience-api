using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartValidationContext
    {     
        public IEnumerable<CartProduct> AllCartProducts { get; set; } = Enumerable.Empty<CartProduct>();
        public IEnumerable<PaymentMethod> AvailPaymentMethods { get; set; } = Enumerable.Empty<PaymentMethod>();
        public IEnumerable<ShippingRate> AvailShippingRates { get; set; } = Enumerable.Empty<ShippingRate>();
    }
}
