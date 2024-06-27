using System;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public abstract class CartCommand : ICartCommand, ICommand<CartAggregate>
    {
        protected CartCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        protected CartCommand(string storeId, string type, string cartName, string userId, string currencyCode, string cultureName)
        {
            StoreId = storeId;
            CartType = type;
            CartName = cartName;
            UserId = userId;
            CurrencyCode = currencyCode;
            CultureName = cultureName;
        }

        public string CartId { get; set; }

        public string StoreId { get; set; }
        public string CartType { get; set; }
        public string CartName { get; set; } = "default";
        public string UserId { get; set; }
        public string OrganizationId { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureName { get; set; }
    }
}
