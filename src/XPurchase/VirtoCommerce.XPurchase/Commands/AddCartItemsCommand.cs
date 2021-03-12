namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartItemsCommand : CartCommand
    {
        public AddCartItemsCommand()
        {
        }

        public AddCartItemsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, NewCartItem[] cartItems)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            CartItems = cartItems;
        }

        public NewCartItem[] CartItems { get; set; }
    }
}
