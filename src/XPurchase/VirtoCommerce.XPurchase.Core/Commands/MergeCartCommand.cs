using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
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

        public bool DeleteAfterMerge { get; set; } = true;
    }
}
