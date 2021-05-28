using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class UpdateOrderPaymentDynamicPropertiesCommand : ICommand<CustomerOrderAggregate>
    {
        public string OrderId { get; set; }

        public string PaymentId { get; set; }

        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
