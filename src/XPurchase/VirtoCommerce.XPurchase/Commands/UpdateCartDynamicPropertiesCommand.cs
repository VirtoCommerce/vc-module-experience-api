using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateCartDynamicPropertiesCommand : CartCommand
    {
        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
