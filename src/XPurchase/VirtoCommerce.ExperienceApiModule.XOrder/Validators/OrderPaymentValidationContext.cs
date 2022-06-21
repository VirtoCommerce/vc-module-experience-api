using System.Collections.Generic;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Validators
{
    public class OrderPaymentValidationContext
    {
        public PaymentIn Payment { get; set; }
        public IEnumerable<PaymentMethod> AvailPaymentMethods { get; set; }
    }
}
