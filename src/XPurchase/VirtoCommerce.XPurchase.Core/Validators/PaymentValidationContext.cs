using System.Collections.Generic;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.XPurchase.Core.Validators
{
    public class PaymentValidationContext
    {
        public Payment Payment { get; set; }
        public IEnumerable<PaymentMethod> AvailPaymentMethods { get; set; }
    }
}
