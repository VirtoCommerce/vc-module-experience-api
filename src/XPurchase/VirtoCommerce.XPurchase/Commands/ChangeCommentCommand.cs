namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCommentCommand : CartCommand
    {
        public ChangeCommentCommand()
        {
        }

        public ChangeCommentCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string comment)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            Comment = comment;
        }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }
    }
}
