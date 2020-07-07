namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateCartCommand : CartCommand
    {
        public CreateCartCommand()
            : base()
        {
        }
        public CreateCartCommand(string storeId, string type, string cartName, string userId, string currency, string language)
            : base(storeId, type, cartName, userId, currency, language)
        {
        }
    }
}
