using System;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public abstract class CartCommand : CartCommandBase, ICommand<CartAggregate>
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
    }
}
