namespace VirtoCommerce.XPurchase.Commands
{
    public class RejectCartItemsCommand : CartCommand
    {
        public RejectCartItemsCommand()
        {
        }

        public RejectCartItemsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string[] giftItemIds)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            GiftItemIds = giftItemIds;
        }

        public string[] GiftItemIds { get; set; }
    }
}
