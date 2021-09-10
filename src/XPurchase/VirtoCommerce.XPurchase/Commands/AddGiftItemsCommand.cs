namespace VirtoCommerce.XPurchase.Commands
{
    public class AddGiftItemsCommand : CartCommand
    {
        public AddGiftItemsCommand()
        {
        }

        public AddGiftItemsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string[] giftItemIds)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            GiftItemIds = giftItemIds;
        }

        public IList<string> GiftItemIds { get; set; }
    }
}
