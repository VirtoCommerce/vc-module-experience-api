using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class UpdateCartItemDynamicPropertiesCommand : CartCommand
    {
        public UpdateCartItemDynamicPropertiesCommand()
        {
        }

        public UpdateCartItemDynamicPropertiesCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string lineItemId, IList<DynamicPropertyValue> dynamicProperties)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            LineItemId = lineItemId;
            DynamicProperties = dynamicProperties;
        }

        public string LineItemId { get; set; }

        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
