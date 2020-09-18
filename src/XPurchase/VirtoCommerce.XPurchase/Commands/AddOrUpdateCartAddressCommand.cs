using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartAddressCommand : CartCommand
    {
        public Address Address { get; set; }
    }
}
