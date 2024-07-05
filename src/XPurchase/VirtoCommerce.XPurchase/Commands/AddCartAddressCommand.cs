using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartAddressCommand : CartCommand
    {
        public AddCartAddressCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public AddCartAddressCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, ExpCartAddress address)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Address = address;
        }

        public ExpCartAddress Address { get; set; }
    }
}
