using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddGiftItemsCommand : CartCommand
    {
        public AddGiftItemsCommand()
        {
        }

        public AddGiftItemsCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, IReadOnlyCollection<string> ids)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Ids = ids;
        }

        /// <summary>
        /// Ids of rewards to add
        /// </summary>
        public IReadOnlyCollection<string> Ids { get; set; }
    }
}
