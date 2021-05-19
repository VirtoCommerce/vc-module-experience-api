using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateCartPaymentDynamicPropertiesCommand : CartCommand
    {
        public string PaymentId { get; set; }

        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
