using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class AddOrUpdateCartAddressCommand : CartCommand
    {
        public ExpCartAddress Address { get; set; }
    }
}
