namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartAddressCommand : CartCommand
    {
        public AddressOptional Address { get; set; }
    }
}
