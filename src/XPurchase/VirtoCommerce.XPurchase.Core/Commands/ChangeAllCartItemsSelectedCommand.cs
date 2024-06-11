using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class ChangeAllCartItemsSelectedCommand : CartCommand
    {
        public bool SelectedForCheckout { get; set; }
    }
}
