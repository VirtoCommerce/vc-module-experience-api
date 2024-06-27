using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCommentCommand : CartCommand
    {
        public ChangeCommentCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
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
