namespace VirtoCommerce.XPurchase.Commands
{
    public class MergeCartCommand : CartCommand
    {
        public MergeCartCommand()
        {
        }

        public MergeCartCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string secondCartId)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            SecondCartId = secondCartId;
        }

        public string SecondCartId { get; set; }
    }
}
