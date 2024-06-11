using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class UpdateCartDynamicPropertiesCommand : CartCommand
    {
        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
