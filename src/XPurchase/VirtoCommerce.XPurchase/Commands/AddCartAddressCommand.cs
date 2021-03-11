using Address = VirtoCommerce.CartModule.Core.Model.Address;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartAddressCommand : CartCommand
    {
        public AddCartAddressCommand()
        {
        }

        public AddCartAddressCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, Address address)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Address = address;
        }

        public Address Address { get; set; }
    }
}
