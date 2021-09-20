using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.XPurchase.Validators
{
    public class PaymentValidationContext
    {
        public Payment Payment { get; set; }
        public IEnumerable<PaymentMethod> AvailPaymentMethods { get; set; }
    }
}
