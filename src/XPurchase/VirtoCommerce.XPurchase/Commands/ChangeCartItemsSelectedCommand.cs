using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemsSelectedCommand : CartCommand
    {
        public IList<string> LineItemIds { get; set; } = new List<string>();

        public bool SelectedForCheckout { get; set; }
    }
}
