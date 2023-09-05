namespace VirtoCommerce.XPurchase
{
    public class ItemSelectedForCheckout
    {
        public string LineItemId { get; }
        public bool SelectedForCheckout { get; }

        public ItemSelectedForCheckout(string lineItemId, bool selectedForCheckout)
        {
            LineItemId = lineItemId;
            SelectedForCheckout = selectedForCheckout;
        }
    }
}
