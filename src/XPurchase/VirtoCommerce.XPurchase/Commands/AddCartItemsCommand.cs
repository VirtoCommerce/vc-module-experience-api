namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartItemsCommand : CartCommand
    {
        public AddCartItemsCommand()
        {
        }

        public AddCartItemsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, NewCartItem[] addCartItems)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            AddCartItems = addCartItems;
        }

        public NewCartItem[] AddCartItems { get; set; }
    }
}
