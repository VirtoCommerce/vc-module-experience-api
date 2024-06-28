using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateCartItemDynamicPropertiesCommand : CartCommand
    {
        public UpdateCartItemDynamicPropertiesCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
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
