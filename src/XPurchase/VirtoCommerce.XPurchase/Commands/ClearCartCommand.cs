namespace VirtoCommerce.XPurchase.Domain.Commands
{
    public class ClearCartCommand : CartCommand
    {
        public ClearCartCommand(string storeId, string cartType, string cartName, string userId, string currency, string language)
            :base(storeId, cartType, cartName, userId, currency, language)
        {
        }                     
    }
}
