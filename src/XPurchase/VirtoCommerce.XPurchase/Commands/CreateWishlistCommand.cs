namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateWishlistCommand : CartCommand
    {
        public string ListName { get; set; }

        public CreateWishlistCommand(string listName, string storeId, string userId, string currencyCode, string cultureName)
        {
            ListName = CartName = listName;
            StoreId = storeId;
            UserId = userId;
            CurrencyCode = currencyCode;
            CultureName = cultureName;
        }
    }
}
