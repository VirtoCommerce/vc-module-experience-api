namespace VirtoCommerce.XPurchase.Commands
{
    public class MergeCartCommand : CartCommand
    {
        public MergeCartCommand()
        {
        }

        public MergeCartCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string secondCartId)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            SecondCartId = secondCartId;
        }

        public string SecondCartId { get; set; }
    }
}
