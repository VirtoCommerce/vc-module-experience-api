using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class AddCartAddressCommand : CartCommand
    {
        public AddCartAddressCommand()
        {
        }

        public AddCartAddressCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, ExpCartAddress address)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Address = address;
        }

        public ExpCartAddress Address { get; set; }
    }
}
