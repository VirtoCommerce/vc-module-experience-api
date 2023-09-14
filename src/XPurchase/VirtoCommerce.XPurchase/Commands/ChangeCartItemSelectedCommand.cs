namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemSelectedCommand : CartCommand
    {
        public string LineItemId { get; set; }

        public bool SelectedForCheckout { get; set; }
    }
}
