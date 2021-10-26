namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartAddressCommand : CartCommand
    {
        public ExpCartAddress Address { get; set; }
    }
}
