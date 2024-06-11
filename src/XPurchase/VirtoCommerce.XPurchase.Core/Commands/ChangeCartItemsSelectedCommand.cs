using System.Collections.Generic;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class ChangeCartItemsSelectedCommand : CartCommand
    {
        public IList<string> LineItemIds { get; set; } = new List<string>();

        public bool SelectedForCheckout { get; set; }
    }
}
