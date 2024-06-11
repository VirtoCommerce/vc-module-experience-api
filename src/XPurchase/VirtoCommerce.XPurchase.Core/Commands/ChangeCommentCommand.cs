using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class ChangeCommentCommand : CartCommand
    {
        public ChangeCommentCommand()
        {
        }

        public ChangeCommentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string comment)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Comment = comment;
        }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }
    }
}
