using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemCommentCommand : CartCommand
    {
        public ChangeCartItemCommentCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
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
