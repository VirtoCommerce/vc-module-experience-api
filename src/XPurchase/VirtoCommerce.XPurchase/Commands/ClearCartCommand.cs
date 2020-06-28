namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearCartCommand : CartCommand
    {
        public ClearCartCommand()
            :base()
        {
        }
        public ClearCartCommand(string storeId, string type, string cartName, string userId, string currency, string language)
            :base(storeId, type, cartName, userId, currency, language)
        {
        }                     
    }
}
