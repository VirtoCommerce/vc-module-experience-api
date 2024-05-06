using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateCartShipmentDynamicPropertiesCommand : CartCommand
    {
        public string ShipmentId { get; set; }

        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
