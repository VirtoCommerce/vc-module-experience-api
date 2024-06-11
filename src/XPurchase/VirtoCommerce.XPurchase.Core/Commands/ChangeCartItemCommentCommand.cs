using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class ChangeCartItemCommentCommand : CartCommand
    {
        public ChangeCartItemCommentCommand()
        {
        }

        public ChangeCartItemCommentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string lineItemId, string comment)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            LineItemId = lineItemId;
            Comment = comment;
        }

        public string LineItemId { get; set; }
        public string Comment { get; set; }
    }
}
