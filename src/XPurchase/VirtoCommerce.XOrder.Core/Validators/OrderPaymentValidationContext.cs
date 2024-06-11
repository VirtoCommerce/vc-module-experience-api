using System.Collections.Generic;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Validators
{
    public class OrderPaymentValidationContext
    {
        public PaymentIn Payment { get; set; }
        public IEnumerable<PaymentMethod> AvailPaymentMethods { get; set; }
    }
}
