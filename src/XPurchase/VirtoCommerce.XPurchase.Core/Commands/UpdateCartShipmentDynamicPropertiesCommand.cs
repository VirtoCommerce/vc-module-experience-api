using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class UpdateCartShipmentDynamicPropertiesCommand : CartCommand
    {
        public string ShipmentId { get; set; }

        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
