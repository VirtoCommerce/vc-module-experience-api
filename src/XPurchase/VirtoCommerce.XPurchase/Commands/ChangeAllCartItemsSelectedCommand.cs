namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeAllCartItemsSelectedCommand : CartCommand
    {
        public bool SelectedForCheckout { get; set; }
    }
}
