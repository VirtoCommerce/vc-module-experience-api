using System.Collections.Generic;

namespace VirtoCommerce.XPurchase
{
    public class ItemSelectedForCheckout
    {
        public IList<string> LineItemIds { get; } = new List<string>();
        public bool SelectedForCheckout { get; }

        public ItemSelectedForCheckout(IList<string> lineItemIds, bool selectedForCheckout)
        {
            LineItemIds = lineItemIds;
            SelectedForCheckout = selectedForCheckout;
        }

        public ItemSelectedForCheckout()
        {
        }
    }
}
