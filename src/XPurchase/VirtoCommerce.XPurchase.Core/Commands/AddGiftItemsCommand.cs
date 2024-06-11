using System.Collections.Generic;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
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
