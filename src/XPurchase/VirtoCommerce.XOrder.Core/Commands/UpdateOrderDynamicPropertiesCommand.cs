using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.XOrder.Core.Commands
{
    public class UpdateOrderDynamicPropertiesCommand : ICommand<CustomerOrderAggregate>
    {
        public string OrderId { get; set; }

        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
