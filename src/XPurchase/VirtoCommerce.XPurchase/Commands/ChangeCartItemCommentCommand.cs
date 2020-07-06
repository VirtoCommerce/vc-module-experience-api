namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemCommentCommand : CartCommand
    {
        public ChangeCartItemCommentCommand()
        {
        }

        public ChangeCartItemCommentCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string lineItemId, int comment)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            LineItemId = lineItemId;
            Comment = comment;
        }

        public string LineItemId { get; set; }
        public string Comment { get; set; }
    }
}
