using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class RemoveCartItemsCommand : CartCommand
    {
        public RemoveCartItemsCommand()
        {
        }

        public RemoveCartItemsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string[] lineItemIds)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            LineItemIds = lineItemIds;
        }

        public string[] LineItemIds { get; set; }
    }
}
