namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemSelectedCommand : CartCommand
    {
        public ChangeCartItemSelectedCommand()
        {
        }

        public ChangeCartItemSelectedCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string lineItemId, bool selectedForCheckout)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            LineItemId = lineItemId;
            SelectedForCheckout = selectedForCheckout;
        }

        public string LineItemId { get; set; }
        public bool SelectedForCheckout { get; set; }
    }
}
