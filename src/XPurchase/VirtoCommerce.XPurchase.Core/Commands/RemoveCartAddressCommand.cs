using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class RemoveCartAddressCommand : CartCommand
    {
        public string AddressId { get; set; }
    }
}
